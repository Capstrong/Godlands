using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ActorStates
{
	Grounded = 0,
	Jumping,
	Falling,
	Rolling,
	Climbing
}

public class ActorPhysics : ActorComponent
{
	private new Transform transform;
	private new Rigidbody rigidbody;

	// States
	#region States
	public delegate void ActorStateMethod();

	[ReadOnlyAttribute, SerializeField]
	ActorStates currentState = ActorStates.Jumping;
	protected ActorStateMethod CurrentStateMethod;
	public Dictionary<ActorStates, ActorStateMethod> stateMethodMap = new Dictionary<ActorStates, ActorStateMethod>();
	#endregion

	#region Movement
	[Header( "Movement" )]
	public Vector3 inputVec = Vector3.zero;
	public float moveSpeedMod = 1f;

	protected Vector3 lastVelocity = Vector3.zero;
	protected Vector3 moveVec = Vector3.zero;

	public float stoppingSpeed = 0.001f;
	protected float currStoppingPower = 0.0f;

	[SerializeField] float stopMoveTime = 0.3f;
	float stopMoveTimer = 0f;

	[SerializeField] float _groundedMoveSpeed = 6f;
	public float groundedMoveSpeed
	{
		get { return _groundedMoveSpeed; }
	}

	[SerializeField] protected float jumpMoveSpeed = 6f;
	[SerializeField] protected float rollMoveSpeed = 6f;
	[SerializeField] protected float climbMoveSpeed = 6f;
	#endregion

	#region Collisions
	[Space( 10 ), Header( "Collisions" )]
	public Collider bumper;
	private Transform _bumperTransform;
	private Rigidbody _bumperRigidbody;
	#endregion

	#region Model Info
	[Space( 10 ), Header( "Model Info" )]
	[SerializeField] float modelTurnSpeed = 7f;
	[Range( 0.0f, 1.0f )]
	[SerializeField] float lookDirLerp = 0.002f;

	private Vector3 _lastPos;
	#endregion

	#region Jumping
	[Space( 10 ), Header( "Jumping" )]
	[SerializeField] public float jumpForce = 8.5f;

	[SerializeField] float jumpCheckDistance = 1.0f;
	[SerializeField] float jumpCheckRadius = 0.5f;

	[SerializeField] LayerMask jumpLayer = 0;
	[SerializeField] float minJumpDot = 0.4f;

	[SerializeField] float jumpColCheckTime = 0.5f;
	float _jumpColCheckTimer = 0.0f;

	[SerializeField] float lateJumpTime = 0.5f;
	bool _lateJump = false;

	[Tooltip( "The minimum delay between jump checks. Use this to prevent the player from immediately colliding with the ground after jumping." )]
	[SerializeField] float jumpCheckDelay = 0.1f;
	bool _jumpCheckDelay = false;

	bool _isOnGround = false;
	#endregion

	#region Rolling
	[Space( 10 ), Header( "Rolling" )]
	[SerializeField] float rollTime = 1f;
	[SerializeField] float rollCooldownTime = 1f;
	float rollCooldownTimer = 0f;
	#endregion

	#region Climbing
	[Space( 10 ), Header( "Climbing" )]
	[SerializeField] LayerMask climbLayer = (LayerMask)0;
	[SerializeField] float climbCheckDistance = 0.5f;
	[SerializeField] float climbCheckRadius = 0.7f;
	[SerializeField] float surfaceHoldForce = 0.1f;

	[SerializeField] float climbCheckTime = 0.2f;
	float climbCheckTimer = 1f;

	Transform climbSurface = null;

	public bool isGrabbing
	{
		get{ return WadeUtils.ValidAxisInput("Grab"); }
	}
	#endregion

	public override void Awake()
	{
		base.Awake();

		transform = GetComponent<Transform>();
		rigidbody = GetComponent<Rigidbody>();

		_lastPos = transform.position;
		currStoppingPower = stoppingSpeed;

		_bumperTransform = bumper.GetComponent<Transform>();
		_bumperRigidbody = bumper.GetComponent<Rigidbody>();

		SetupStateMethodMap();
	}

	void FixedUpdate()
	{
		// Pre-Update stuff
		FollowBumper();
		GroundedCheck();

		// Update
		CurrentStateMethod();

		// Post-Update stuff
		ConstrainBumper();
		OrientSelf();
		_isOnGround = false;
	}

	void ConstrainBumper()
	{
		_bumperTransform.position = transform.position;
		_bumperRigidbody.velocity = rigidbody.velocity;
	}

	void FollowBumper()
	{
		if ( !IsInState( ActorStates.Climbing ) )
		{
			Vector3 constrainedPos = transform.position;
			constrainedPos.x = _bumperTransform.position.x;
			constrainedPos.z = _bumperTransform.position.z;
			transform.position = constrainedPos;
		}
	}

	public virtual void SetupStateMethodMap() { }

	public void MoveAtSpeed( Vector3 inVec, float appliedMoveSpeed )
	{
		rigidbody.useGravity = true;
		
		currStoppingPower = stoppingSpeed;

		moveVec = inVec * appliedMoveSpeed * moveSpeedMod;
		moveVec.y = rigidbody.velocity.y;

		lastVelocity = moveVec;
		rigidbody.velocity = moveVec;

		if ( actor.animator != null )
		{
			actor.animator.SetBool( "isMoving", true );
		}
	}

	public void ComeToStop()
	{
		currStoppingPower -= Time.deltaTime;
		currStoppingPower = Mathf.Clamp( currStoppingPower, 0.0f, stoppingSpeed );

		//CheckGroundSlope();
		moveVec = rigidbody.velocity * stoppingSpeed;// * groundSlopeSpeedMod;

		moveVec.y = rigidbody.velocity.y;
		rigidbody.velocity = moveVec;

		if ( actor.animator ) actor.animator.SetBool( "isMoving", false );

		if ( _jumpColCheckTimer > jumpColCheckTime )
		{
			if ( IsInState( ActorStates.Grounded ) )
			{
				if ( stopMoveTimer >= stopMoveTime )
				{
					rigidbody.useGravity = false;
					SetFallSpeed( 0.0f );
				}

				stopMoveTimer += Time.deltaTime;
			}
			else
			{
				rigidbody.useGravity = true;
			}
		}
	}

	public void ClimbCheck()
	{
		if (isGrabbing && ( ( actor as PlayerActor ) == null || ( actor as PlayerActor ).actorStats.CanUseStamina() ) )
		{
			if (climbCheckTimer > climbCheckTime)
			{
				RaycastHit hit;
				Physics.SphereCast( new Ray( transform.position, transform.forward ), climbCheckRadius, out hit, climbCheckDistance, climbLayer );
				if ( hit.transform )
				{
					StartClimbing( hit.collider );
				}
				else
				{
					Collider[] cols = Physics.OverlapSphere( transform.position, climbCheckRadius, climbLayer );
					if ( cols.Length > 0 )
					{
						Collider nearestCol = cols[0];
						foreach ( Collider col in cols )
						{
							if ( ( col.transform.position - transform.position ).sqrMagnitude < ( nearestCol.transform.position - transform.position ).sqrMagnitude )
							{
								nearestCol = col;
							}
						}

						StartClimbing( nearestCol );
					}
					else
					{
						StopClimbing();
					}
				}

				climbCheckTimer = 0f;
			}
		}
		else if ( climbSurface )
		{
			StopClimbing();
		}
		
		climbCheckTimer += Time.fixedDeltaTime;
	}

	public void ClimbSurface()
	{
		if ( climbSurface )
		{
			ClimbableTag climbTag = climbSurface.GetComponent<ClimbableTag>();
			if ( climbTag )
			{
				rigidbody.useGravity = false;
				currStoppingPower = stoppingSpeed;

				Vector3 surfaceRelativeInput = climbSurface.InverseTransformDirection( inputVec );
				surfaceRelativeInput = new Vector3( surfaceRelativeInput.x,
				                                    surfaceRelativeInput.z,
				                                    surfaceRelativeInput.y );

				if ( !climbTag.xMovement ) surfaceRelativeInput.x = 0f;
				if ( !climbTag.yMovement ) surfaceRelativeInput.y = 0f;

				Vector3 surfaceHoldVec = climbSurface.forward * surfaceHoldForce;
				moveVec = climbSurface.rotation * surfaceRelativeInput * climbMoveSpeed * moveSpeedMod;
				moveVec += surfaceHoldVec;

				lastVelocity = moveVec;
				rigidbody.velocity = moveVec;

				transform.rotation = Quaternion.Lerp(
				    transform.rotation,
				    Quaternion.LookRotation( climbSurface.forward, Vector3.up ),
				    Time.deltaTime * modelTurnSpeed );
			}
			else
			{
				Debug.LogError("Cannot climb, surface isn't tagged. This is probably a problem in ClimbCheck");
			}
		}
		else
		{
			Debug.LogError("Cannot climb, surface is null");
		}
	}

	public void StartClimbing( Collider col )
	{
		( actor as PlayerActor).actorStats.StartUsingStamina();

		climbSurface = col.transform;
		ChangeState( ActorStates.Climbing );

		if ( actor.animator != null )
		{
			actor.animator.SetBool( "isClimbing", true );
		}
	}
	
	public void StopClimbing()
	{
		if ( climbSurface )
		{
			rigidbody.useGravity = true;
			climbSurface = null;
			ChangeState( ActorStates.Jumping );

			( actor as PlayerActor ).actorStats.StopUsingStamina();

			if ( actor.animator != null )
			{
				actor.animator.SetBool( "isClimbing", false );
			}
		}
	}

	public void RollCheck()
	{
		if ( IsInState( ActorStates.Rolling ) )
		{
			if ( rollCooldownTimer >= rollTime )
			{
				ChangeState( ActorStates.Jumping );
				actor.animator.SetBool( "isRolling", false );
			}
		}
		else if ( Input.GetButtonDown( "Roll" + WadeUtils.platformName ) &&
		          rollCooldownTimer >= rollCooldownTime && inputVec.magnitude > WadeUtils.SMALLNUMBER )
		{
			ChangeState( ActorStates.Rolling );
			actor.animator.SetBool( "isRolling", true );

			rollCooldownTimer = 0f;
		}

		rollCooldownTimer += Time.deltaTime;
	}

	public void JumpCheck()
	{
		if ( _isOnGround || _lateJump )
		{
			Vector3 curVelocity = rigidbody.velocity;
			curVelocity.y = jumpForce;
			rigidbody.velocity = curVelocity;
			rigidbody.useGravity = true;

			_jumpColCheckTimer = 0.0f;
			actor.animator.SetBool( "isJumping", true );
			//actor.GetAnimator().SetBool("isSliding", false);

			ChangeState( ActorStates.Jumping );
			StartJumpCheckDelayTimer();
		}
	}

	protected void ChangeState( ActorStates toState )
	{
		currentState = toState;
		CurrentStateMethod = stateMethodMap[currentState];
	}

	void GroundedCheck()
	{
		RaycastHit hit;
		Physics.SphereCast( transform.position + Vector3.up,
		                    jumpCheckRadius,
		                    -Vector3.up * jumpCheckDistance,
		                    out hit,
		                    jumpCheckDistance,
		                    jumpLayer );
		_isOnGround = ( hit.transform &&
		                Vector3.Dot( hit.normal, Vector3.up ) > minJumpDot );

		if ( _isOnGround &&
		     !_jumpCheckDelay &&
		     !IsInState( ActorStates.Climbing ) )
		{
			if ( !IsInState( ActorStates.Grounded ) && !IsInState( ActorStates.Rolling ) )
			{
				ChangeState( ActorStates.Grounded );
				stopMoveTimer = 0f;
			}

			actor.animator.SetBool( "isJumping", false );
			CancelLateJumpTimer();
			//actor.GetAnimator().SetBool("isSliding", false);
		}
		else if ( IsInState( ActorStates.Grounded ) )
		{
			if ( actor.animator != null )
			{
				actor.animator.SetBool( "isJumping", true );
				//actor.GetAnimator().SetBool("isSliding", false);
			}

			// start falling
			StartLateJumpTimer();

			ChangeState( ActorStates.Falling );
		}
	}

	/**
	 * Orients the model to face the direction of movement
	 *
	 * Orientation is reactive to velocity, meaning the
	 * model will face the direction of movement, rather
	 * than the input direction.
	 */
	void OrientSelf()
	{
		Vector3 actualVelocity = transform.position - _lastPos;
		actualVelocity.y = 0.0f;

		Vector3 intendedVelocity = rigidbody.velocity;
		intendedVelocity.y = 0.0f;

		Vector3 lookVec = Vector3.Lerp( actualVelocity, intendedVelocity, lookDirLerp );

		if ( !lookVec.IsZero() )
		{
			transform.rotation = Quaternion.Lerp( transform.rotation,
			                                  Quaternion.LookRotation( lookVec, transform.up ),
			                                  Time.deltaTime * modelTurnSpeed );
		}

		_lastPos = transform.position;
	}

	bool IsInState( ActorStates checkState )
	{
		return currentState == checkState;
	}

	void SetFallSpeed( float fallSpeed )
	{
		Vector3 moveVec = rigidbody.velocity;
		moveVec.y = fallSpeed;
		rigidbody.velocity = moveVec;
	}

	#region Late Jump Timer
	void StartLateJumpTimer()
	{
		_lateJump = true;
		Invoke( "EndLateJump", lateJumpTime );
	}

	void CancelLateJumpTimer()
	{
		if ( IsInvoking( "EndLateJump" ) )
		{
			CancelInvoke( "EndLateJump" );
		}
		EndLateJump();
	}

	void EndLateJump()
	{
		_lateJump = false;
	}
	#endregion

	#region Jump Check Delay Timer
	void StartJumpCheckDelayTimer()
	{
		_jumpCheckDelay = true;
		Invoke( "EndJumpCheckDelay", jumpCheckDelay );
	}

	void CancelJumpCheckDelayTimer()
	{
		if ( IsInvoking( "EndJumpCheckDelay" ) )
		{
			CancelInvoke( "EndJumpCheckDelay" );
		}
		EndJumpCheckDelay();
	}

	void EndJumpCheckDelay()
	{
		_jumpCheckDelay = false;
	}
	#endregion

	void OnDrawGizmosSelected()
	{
		if ( Application.isPlaying )
		{
			//Gizmos.DrawWireSphere( GetComponent<Transform>().position + bumper.center,
			//					   bumper.radius );
		}
	}
}
