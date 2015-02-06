using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ActorStates
{
	Grounded = 0,
	Jumping,
	Rolling,
	Climbing
}

public class ActorPhysics : ActorComponent
{
	private new Transform transform;

	// States
	#region States
	public delegate void ActorStateMethod();

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
	public SphereCollider lifter;
	public SphereCollider bumper;
	private Transform _bumperTransform;
	private Rigidbody _bumperRigidbody;

	[Tooltip( "Force used to negate lift created by the bumper" )]
	public float bumpForce = 1.0f;

	public float velocityLerp = 0.5f;

	private float _lastY;
	#endregion

	#region Model Info
	[Space( 10 ), Header( "Model Info" )]
	public Transform model;
	[SerializeField] float modelTurnSpeed = 7f;
	protected Vector3 modelOffset;
	#endregion

	#region Jumping
	[Space( 10 ), Header( "Jumping" )]
	[SerializeField] public float jumpForce = 8.5f;

	[SerializeField] float jumpCheckDistance = 1.0f;
	[SerializeField] Vector3 groundPosOffset = new Vector3(0f, -1f, 0f);

	[SerializeField] LayerMask jumpLayer = 0;
	[SerializeField] float minJumpDot = 0.4f;

	[SerializeField] float jumpColCheckTime = 0.5f;
	float jumpColCheckTimer = 0.0f;
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

		modelOffset = transform.position - model.position;
		currStoppingPower = stoppingSpeed;

		SetupStateMethodMap();

		_bumperTransform = bumper.GetComponent<Transform>();
		_bumperRigidbody = bumper.GetComponent<Rigidbody>();
		_bumperTransform.parent = null;
		_lastY = transform.position.y;
	}

	void FixedUpdate()
	{
		FollowBumper();

		CurrentStateMethod();
		_lastY = transform.position.y;

		ConstrainBumper();
	}

	//void OnCollisionStay( Collision collision )
	//{
	//	bool lifterCollision = false;
	//	bool bumperCollision = false;
	//	foreach ( ContactPoint contact in collision.contacts )
	//	{
	//		if ( contact.point.y > transform.position.y + bumper.center.y - bumper.radius )
	//		{
	//			Vector3 negationForce = new Vector3( rigidbody.velocity.x,
	//												 -contact.normal.y,
	//												 rigidbody.velocity.z );

	//			Debug.DrawRay( contact.point, negationForce.normalized * bumpForce * collision.relativeVelocity.magnitude );
	//			Debug.DrawRay( contact.point, contact.normal );

	//			//rigidbody.velocity = negationForce;

	//			bumperCollision = true;
	//		}
	//		else
	//		{
	//			lifterCollision = true;
	//		}

	//		// Only override y if the bumper collided but the lifter didn't
	//		// The case being the player is going up an incline while
	//		// rubbing along a wall, so the lifter should be lifting them
	//		// up the incline and the bumper shouldn't negate that
	//		if ( bumperCollision && !lifterCollision )
	//		{
	//			//transform.SetPositionY( _lastY );
	//		}
	//	}
	//}

	void ConstrainBumper()
	{
		_bumperTransform.position = transform.position;
		_bumperRigidbody.velocity = rigidbody.velocity;
	}

	void FollowBumper()
	{
		Vector3 constrainedPos = transform.position;
		constrainedPos.x = _bumperTransform.position.x;
		constrainedPos.z = _bumperTransform.position.z;
		transform.position = constrainedPos;
	}

	public virtual void SetupStateMethodMap() { }

	public void MoveAtSpeed( Vector3 inVec, float appliedMoveSpeed )
	{
		rigidbody.useGravity = true;
		
		currStoppingPower = stoppingSpeed;
		
		//CheckGroundSlope();
		
		moveVec = inVec * appliedMoveSpeed * moveSpeedMod;// * groundSlopeSpeedMod;
		moveVec.y = rigidbody.velocity.y;

		lastVelocity = moveVec;
		rigidbody.velocity = moveVec;

		if ( actor.animator != null )
		{
			actor.animator.SetBool( "isMoving", true );
		}
		
		ModelControl();
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

		if ( jumpColCheckTimer > jumpColCheckTime )
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

		ModelControl();
	}

	public void ClimbCheck()
	{
		if (isGrabbing && ( ( actor as PlayerActor ) == null || ( actor as PlayerActor ).actorStats.CanUseStamina() ) )
		{
			if (climbCheckTimer > climbCheckTime)
			{
				RaycastHit hit;
				Physics.SphereCast( new Ray( transform.position, model.forward ), climbCheckRadius, out hit, climbCheckDistance, climbLayer );
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
		if(climbSurface)
		{
			ClimbableTag ct = climbSurface.GetComponent<ClimbableTag>();
			if(ct)
			{
				rigidbody.useGravity = false;
				currStoppingPower = stoppingSpeed;

				Vector3 surfaceRelativeInput = climbSurface.InverseTransformDirection( inputVec );
				surfaceRelativeInput = new Vector3( surfaceRelativeInput.x, 
				                                    surfaceRelativeInput.z, 
				                                    surfaceRelativeInput.y );

				if ( !ct.xMovement ) surfaceRelativeInput.x = 0f;
				if ( !ct.yMovement ) surfaceRelativeInput.y = 0f;

				Vector3 surfaceHoldVec = climbSurface.forward * surfaceHoldForce;
				moveVec = climbSurface.rotation * surfaceRelativeInput * climbMoveSpeed * moveSpeedMod;
				moveVec += surfaceHoldVec;

				lastVelocity = moveVec;
				rigidbody.velocity = moveVec;

				model.rotation = Quaternion.Lerp( model.rotation, 
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
		if(climbSurface)
		{
			rigidbody.useGravity = true;
			climbSurface = null;
			ChangeState( ActorStates.Jumping );

			(actor as PlayerActor).actorStats.StopUsingStamina();

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
		bool isOnGround = false;

		RaycastHit hit;
		Physics.Raycast( transform.position + Vector3.up,
		                 -Vector3.up,
		                 out hit,
		                 jumpCheckDistance,
		                 jumpLayer );
		if ( hit.transform && Vector3.Dot( hit.normal, Vector3.up ) > minJumpDot )
		{
			isOnGround = true;
		}

		if ( isOnGround )
		{
			//if ( Input.GetButtonDown( "Jump" + WadeUtils.platformName ) )
			{
				Vector3 curVelocity = rigidbody.velocity;
				curVelocity.y = jumpForce;
				rigidbody.velocity = curVelocity;
				rigidbody.useGravity = true;

				jumpColCheckTimer = 0.0f;
				//actor.GetAnimator().SetBool("isSliding", false);
			}
			//else if ( jumpColCheckTimer > jumpColCheckTime && isOnGround && !IsInState( ActorStates.Rolling ) )
			//{
			//	if ( !IsInState( ActorStates.Grounded ) )
			//	{
			//		// if !launching
			//		ChangeState( ActorStates.Grounded );
			//		stopMoveTimer = 0f;
			//		//GainControl();
			//	}

			//	actor.animator.SetBool( "isJumping", false );
			//	//actor.GetAnimator().SetBool("isSliding", false);
			//	lateJumpTimer = 0.0f;
			//}
		}
		//else if ( !IsInState( ActorStates.Jumping ) && !IsInState( ActorStates.Rolling ) ) // if not currently being launched
		//{
		//	Debug.Log( "Not on Ground" );

		//	if ( actor.animator != null )
		//	{
		//		actor.animator.SetBool( "isJumping", true );
		//		//actor.GetAnimator().SetBool("isSliding", false);
		//	}

		//	ChangeState( ActorStates.Jumping );
		//}

		if ( !IsInState( ActorStates.Grounded ) )
		{
			jumpColCheckTimer += Time.deltaTime;
		}
	}

	public void ModelControl()
	{
		model.position = transform.position - modelOffset;

		Vector3 lookVec = inputVec;
		lookVec.y = 0.0f;

		if ( !WadeUtils.IsZero( lookVec ) )
		{
			model.rotation = Quaternion.Lerp( model.rotation,
			                                  Quaternion.LookRotation( lookVec, transform.up ),
			                                  Time.deltaTime * modelTurnSpeed );
		}
	}

	//protected void CheckGroundSlope()
	//{
	//	float slopeSpeedMod = 1f;

	//	RaycastHit hit;
	//	Vector3 groundCheckPos = transform.position - Vector3.up * groundSlopeCheckHeight;

	//	Vector3 capsuleRightOffset = model.right * groundSlopeCapsuleWidth/2f;
	//	Physics.CapsuleCast(groundCheckPos + capsuleRightOffset, groundCheckPos - capsuleRightOffset, 
	//						groundSlopeCheckRadius, model.forward, out hit, groundSlopeCastDist);

	//	if ( hit.transform )
	//	{
	//		/// Slope mode is based on the steepness of the surface normal
	//		float groundDot = Vector3.Dot( Vector3.up, hit.normal );
	//		slopeSpeedMod = Mathf.InverseLerp( slopeLimits.min, slopeLimits.max, groundDot ); // Not sure if I need this clamp
	//		slopeSpeedMod = Mathf.Lerp( 0f, 1f, slopeSpeedMod );

	//		if ( slopeSpeedMod < 0.5f )
	//		{
	//			SetFallSpeed( Time.deltaTime * groundSlopeFallSpeed );
	//		}
	//	}

	//	groundSlopeSpeedMod = Mathf.MoveTowards( groundSlopeSpeedMod, slopeSpeedMod, Time.deltaTime * groundSlopeSpeedModeDelta );
	//}

	protected void ChangeState( ActorStates toState )
	{
		currentState = toState;
		CurrentStateMethod = stateMethodMap[currentState];
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

	//void OnDrawGizmosSelected()
	//{
	//	// ColliderVis
	//	Gizmos.color = Color.white;
	//	Gizmos.DrawWireSphere( transform.position + Vector3.up * 0.5f, 0.5f );
	//	Gizmos.DrawWireSphere( transform.position - Vector3.up * 0.5f, 0.5f );

	//	// JumpCheck
	//	Gizmos.color = Color.red;
	//	Gizmos.DrawWireSphere( transform.position + groundPosOffset, jumpCheckRadius );

	//	// SlopeCheck
	//	Gizmos.color = Color.blue;
	//	Gizmos.DrawSphere( transform.position - Vector3.up * groundSlopeCheckHeight + model.forward * 0.3f + model.right * 0.2f, groundSlopeCheckRadius );
	//	Gizmos.DrawSphere( transform.position - Vector3.up * groundSlopeCheckHeight + model.forward * 0.3f - model.right * 0.2f, groundSlopeCheckRadius );

	//	// Climb check
	//	Gizmos.color = Color.green;
	//	Gizmos.DrawSphere( transform.position, climbCheckRadius );
	//	Gizmos.DrawSphere( transform.position + model.forward * climbCheckDistance, climbCheckRadius );
	//}
}
