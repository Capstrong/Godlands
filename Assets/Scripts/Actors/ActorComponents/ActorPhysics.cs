using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ActorStates
{
	Grounded = 0,
	Jumping,
	Rolling
}

public class ActorPhysics : ActorComponent
{
	public delegate void ActorStateMethod();

	ActorStates currentState = ActorStates.Jumping;

	protected ActorStateMethod CurrentStateMethod;

	public Vector3 inputVec = Vector3.zero;

	public Dictionary<ActorStates, ActorStateMethod> stateMethodMap = new Dictionary<ActorStates, ActorStateMethod>();

	// Movement
	protected float dualInputMod = 0.7071f;
	[SerializeField] float maxSpeed = 40f;

	[SerializeField] float _groundedMoveSpeed = 6f;
	public float groundedMoveSpeed
	{
		get { return _groundedMoveSpeed; }
	}

	[SerializeField] protected float jumpMoveSpeed = 6f;
	[SerializeField] protected float rollMoveSpeed = 6f;

	public float moveSpeedMod = 1f;

	protected Vector3 lastVelocity = Vector3.zero;
	protected Vector3 moveVec = Vector3.zero;

	[SerializeField, Range( 0.001f, 1000f )] protected float stoppingSpeed = 0.001f;
	protected float currStoppingPower = 0.0f;

	// Slope limit

	protected float groundSlopeSpeedMod = 1f;
	[SerializeField] MinMaxF slopeLimits = new MinMaxF( 0.28f, 0.72f );
	[SerializeField] float groundSlopeCheckRadius = 0.2f;
	[SerializeField] float groundSlopeRayHeight = 0.7f;

	// Jumping
	public float jumpForce = 8.5f;

	[SerializeField] float jumpCheckDistance = 1.3f;
	[SerializeField] Vector3 groundPosOffset = new Vector3(0f, -1f, 0f);
	
	[SerializeField] float jumpCheckRadius = 1.2f;
	[SerializeField] LayerMask jumpLayer = 0;
	[SerializeField] float minJumpDot = 0.4f;

	[SerializeField] float jumpColCheckTime = 0.5f;
	float jumpColCheckTimer = 0.0f;

	[SerializeField] float lateJumpTime = 0.2f;
	float lateJumpTimer = 0.0f;

	[SerializeField] float stopMoveTime = 0.3f;
	float stopMoveTimer = 0f;

	// Rolling
	[SerializeField] float rollTime = 1f;
	[SerializeField] float rollCooldownTime = 1f;
	float rollCooldownTimer = 0f;

	public bool isGrabbing
	{
		get{ return WadeUtils.ValidAxisInput("Grab"); }
	}

	[SerializeField] float slideTurnSpeed = 7f;
	public Vector3 rigVelocity; // used to visualize velocity in inspector

	public Transform model;
	protected Vector3 modelOffset;

	public override void Awake()
	{
		base.Awake();

		modelOffset = transform.position - model.position;
		currStoppingPower = stoppingSpeed;
	}

	protected void ChangeState( ActorStates toState )
	{
		currentState = toState;
		CurrentStateMethod = stateMethodMap[currentState];
	}

	bool IsInState( ActorStates checkState )
	{
		return currentState == checkState;
	}

	public void ComeToStop()
	{
		currStoppingPower -= Time.deltaTime;
		currStoppingPower = Mathf.Clamp( currStoppingPower, 0.0f, stoppingSpeed );

		CheckGroundSlope();
		moveVec = lastVelocity * currStoppingPower / stoppingSpeed * groundSlopeSpeedMod;

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
	}

	public void MoveAtSpeed( Vector3 inputVec, float appliedMoveSpeed )
	{
		rigidbody.useGravity = true;

		currStoppingPower = stoppingSpeed;
	
		CheckGroundSlope();

		moveVec = inputVec * appliedMoveSpeed * moveSpeedMod * groundSlopeSpeedMod;
		moveVec.y = rigidbody.velocity.y;

		lastVelocity = moveVec;
		rigidbody.velocity = moveVec;

		if ( actor.animator != null )
		{
			actor.animator.SetBool( "isMoving", true );
		}

		ModelControl();
	}

	protected void CheckGroundSlope()
	{
		float slopeSpeedMod = 1f;

		RaycastHit hit;
		Vector3 groundCheckPos = transform.position - Vector3.up * groundSlopeRayHeight;
		Physics.CapsuleCast(groundCheckPos + model.right * 0.2f, groundCheckPos - model.right * 0.2f, 
		                    groundSlopeCheckRadius, model.forward, out hit, 0.3f);

		if( hit.transform )
		{
			/// Slope mode is based on the steepness of the surface normal
			float groundDot = Vector3.Dot( Vector3.up, hit.normal );
			slopeSpeedMod = Mathf.InverseLerp( slopeLimits.min, slopeLimits.max, groundDot ); // Not sure if I need this clamp
			slopeSpeedMod = Mathf.Lerp( 0f, 1f, slopeSpeedMod );

			if( slopeSpeedMod < 0.5f )
			{
				Vector3 curMoveVec = rigidbody.velocity;
				curMoveVec.y -= Time.deltaTime * 30f;
				rigidbody.velocity = curMoveVec;
			}
		}

		groundSlopeSpeedMod = Mathf.MoveTowards( groundSlopeSpeedMod, slopeSpeedMod, Time.deltaTime * 2.5f );
	}

	void SetFallSpeed( float fallSpeed )
	{
		Vector3 moveVec = rigidbody.velocity;
		moveVec.y = fallSpeed;
		rigidbody.velocity = moveVec;
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
		Physics.SphereCast( new Ray( transform.position, -Vector3.up ), jumpCheckRadius, out hit, jumpCheckDistance, jumpLayer );
		if( hit.transform && Vector3.Dot(hit.normal, Vector3.up) > minJumpDot )
		{
			isOnGround = true;
		}

		if ( ( isOnGround || lateJumpTimer < lateJumpTime ) )
		{
			if ( Input.GetButtonDown( "Jump" + WadeUtils.platformName ) )
			{
				Vector3 curVelocity = rigidbody.velocity;
				curVelocity.y = jumpForce;
				rigidbody.velocity = curVelocity;
				rigidbody.useGravity = true;

				jumpColCheckTimer = 0.0f;
				//actor.GetAnimator().SetBool("isSliding", false);
			}
			else if ( jumpColCheckTimer > jumpColCheckTime && isOnGround && !IsInState( ActorStates.Rolling ) )
			{
				if ( !IsInState( ActorStates.Grounded ) )
				{
					// if !launching
					ChangeState( ActorStates.Grounded );
					stopMoveTimer = 0f;
					//GainControl();
				}

				actor.animator.SetBool( "isJumping", false );
				//actor.GetAnimator().SetBool("isSliding", false);
				lateJumpTimer = 0.0f;
			}
		}
		else if ( !IsInState( ActorStates.Jumping ) && !IsInState( ActorStates.Rolling ) ) // if not currently being launched
		{
			if ( actor.animator != null )
			{
				actor.animator.SetBool( "isJumping", true );
				//actor.GetAnimator().SetBool("isSliding", false);
			}

			ChangeState( ActorStates.Jumping );
		}

		if ( !IsInState( ActorStates.Grounded ) )
		{
			jumpColCheckTimer += Time.deltaTime;
		}

		lateJumpTimer += Time.deltaTime;
		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
	}

	public void ModelControl()
	{
		model.position = transform.position - modelOffset;

		Vector3 lookVec = inputVec;
		lookVec.y = 0.0f;

		if(lookVec != Vector3.zero)
		{
			model.rotation = Quaternion.LookRotation(lookVec * 10.0f, transform.up);
		}
	}

	void SlideModelControl()
	{
		model.position = transform.position - modelOffset; // this might be important so don't delete it

		Vector3 lookVec = rigidbody.velocity.normalized;
		lookVec.y = 0.0f;

		if ( lookVec != Vector3.zero )
		{
			model.rotation = Quaternion.Lerp( model.rotation,
			                                  Quaternion.LookRotation( lookVec * 10.0f, transform.up ),
			                                  Time.deltaTime * slideTurnSpeed );
		}
	}

	public void Stop()
	{
		rigidbody.velocity = Vector3.zero;
	}

	void OnLandFromLaunch()
	{
		ChangeState( ActorStates.Grounded );
	}

	void OnDrawGizmos()
	{
		// ColliderVis
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere( transform.position + Vector3.up * 0.5f, 0.5f );
		Gizmos.DrawWireSphere( transform.position - Vector3.up * 0.5f, 0.5f );

		// JumpCheck
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere( transform.position + groundPosOffset, jumpCheckRadius );

		// SlopeCheck
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere( transform.position - Vector3.up * groundSlopeRayHeight + model.forward * 0.3f + model.right * 0.2f, groundSlopeCheckRadius );
		Gizmos.DrawSphere( transform.position - Vector3.up * groundSlopeRayHeight + model.forward * 0.3f - model.right * 0.2f, groundSlopeCheckRadius );
	}


}
