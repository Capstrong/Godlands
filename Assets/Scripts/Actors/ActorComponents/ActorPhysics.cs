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

	[ReadOnly, SerializeField]
	ActorStates currentState = ActorStates.Jumping;
	protected ActorStateMethod CurrentStateMethod;
	public Dictionary<ActorStates, ActorStateMethod> stateMethodMap = new Dictionary<ActorStates, ActorStateMethod>();
	#endregion

	#region Movement
	[Header( "Movement" )]
	public Vector3 inputVec = Vector3.zero;
	public float moveSpeedMod = 1f;

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
	public Collider lifter;
	public Collider climbBumper;
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
	[SerializeField] float climbCheckRadius = 0.7f;
	[Range( 0.0f, 1.0f )]
	[SerializeField] float leanTowardsSurface = 0.5f;

	[SerializeField] float climbCheckTime = 0.2f;
	float climbCheckTimer = 1f;

	Transform climbSurface = null;
	ClimbableTag climbTag;
	Vector3 climbSurfaceNormal = Vector3.zero;
	Vector3 climbSurfaceRight = Vector3.zero;
	Vector3 climbSurfaceUp = Vector3.zero;
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

	public bool ClimbCheck()
	{
		climbCheckTimer += Time.deltaTime;

		bool climbing = false;
		//if ( climbCheckTimer > climbCheckTime )
		{
			Collider[] colliders = Physics.OverlapSphere( transform.position, climbCheckRadius, climbLayer );
			if ( colliders.Length > 0 )
			{
				Collider nearestCol = colliders[0];
				float shortestDistance = float.MaxValue;
				foreach ( Collider col in colliders )
				{
					float distance =  ( col.transform.position - transform.position ).sqrMagnitude;
					if ( distance < shortestDistance )
					{
						nearestCol = col;
						shortestDistance = distance;
					}
				}

				StartClimbing( nearestCol );
				climbing = true;
			}
			else
			{
				StopClimbing();
				climbing = false;
			}

			climbCheckTimer = 0f;
		}
		//else
		//{
		//	climbing = true;
		//}
		
		return climbing;
	}

	public void ClimbSurface( Vector3 movement )
	{
		DebugUtils.Assert( climbSurface, "Cannot climb, surface is null" );

		if ( climbTag )
		{
			DebugUtils.Assert( climbTag.xMovement || climbTag.yMovement );

			currStoppingPower = stoppingSpeed;

			Vector3 surfaceRelativeInput =
			    climbSurfaceRight * ( climbTag.xMovement ? movement.x : 0.0f ) +
			    climbSurfaceUp * ( climbTag.yMovement ? movement.z : 0.0f );

			Debug.DrawRay( transform.position, surfaceRelativeInput * 10.0f );

			moveVec = surfaceRelativeInput * climbMoveSpeed * moveSpeedMod;

			transform.Translate( moveVec * Time.deltaTime, Space.World );
		}
		else
		{
			Debug.LogError( "Cannot climb, surface isn't tagged. This is probably a problem in ClimbCheck" );
		}
	}

	void StartClimbing( Collider col )
	{
		climbSurface = col.transform;
		climbTag = climbSurface.GetComponent<ClimbableTag>();

		if ( Vector3.Dot( climbSurface.forward, ( transform.position - climbSurface.position ) ) > 0.0f )
		{
			climbSurfaceNormal = -climbSurface.forward;
			climbSurfaceRight = -climbSurface.right;
		}
		else
		{
			climbSurfaceNormal = climbSurface.forward;
			climbSurfaceRight = climbSurface.right;
			Debug.LogWarning( "Climb volume is facing into wall." );
		}

		if ( Vector3.Dot( Vector3.up, climbSurface.up ) > 0.0f )
		{
			climbSurfaceUp = climbSurface.up;
		}
		else
		{
			climbSurfaceUp = -climbSurface.up;
			Debug.LogWarning( "Climb volume is upside down." );
		}

		ChangeState( ActorStates.Climbing );
		
		rigidbody.useGravity = false;
		rigidbody.velocity = Vector3.zero;
		lifter.gameObject.SetActive( false );
		bumper.gameObject.SetActive( false );
		climbBumper.gameObject.SetActive( true );

		if ( actor.animator != null )
		{
			actor.animator.SetBool( "isClimbing", true );
		}
	}
	
	public void StopClimbing()
	{
		if ( climbSurface )
		{
			climbSurface = null;
			climbTag = null;
			rigidbody.useGravity = true;
			lifter.gameObject.SetActive( true );
			bumper.gameObject.SetActive( true );
			climbBumper.gameObject.SetActive( false );

			ChangeState( ActorStates.Jumping );

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
		          rollCooldownTimer >= rollCooldownTime && !inputVec.IsZero() )
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
		Quaternion desiredLook = transform.rotation;
		if ( IsInState( ActorStates.Climbing ) )
		{
			Vector3 lookVector = climbSurfaceNormal;
			lookVector.y *= leanTowardsSurface;
			desiredLook = Quaternion.LookRotation( lookVector );
		}
		else
		{
			Vector3 actualVelocity = transform.position - _lastPos;
			actualVelocity.y = 0.0f;

			Vector3 intendedVelocity = rigidbody.velocity;
			intendedVelocity.y = 0.0f;

			Vector3 lookVec = Vector3.Lerp( actualVelocity, intendedVelocity, lookDirLerp );

			if ( !lookVec.IsZero() )
			{
				desiredLook = Quaternion.LookRotation( lookVec, transform.up );
			}
		}

		transform.rotation = Quaternion.Lerp(
		    transform.rotation,
		    desiredLook,
		    Time.deltaTime * modelTurnSpeed );

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
