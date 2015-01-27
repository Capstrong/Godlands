using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ActorStates
{
	Grounded		= 0,
	Jumping,
	Rolling
}

public class ActorPhysics : ActorComponent
{
	ActorStates currentState = ActorStates.Jumping;

	delegate void ActorStateMethod();
	ActorStateMethod CurrentStateMethod;

	Dictionary<ActorStates, ActorStateMethod> stateMethodMap = new Dictionary<ActorStates, ActorStateMethod>();

	// Movement
	[SerializeField] float maxSpeed = 40f;
	[SerializeField] float groundedMoveSpeed = 8.5f;
	[SerializeField] float jumpMoveSpeed = 8.5f;
	[SerializeField] float rollMoveSpeed = 6f;

	public float moveSpeedMod = 1f;

	Vector3 lastVelocity = Vector3.zero;
	Vector3 inputVec = Vector3.zero;
	Vector3 moveVec = Vector3.zero;
	
	[Range (0.001f, 1000f)][SerializeField] float stoppingSpeed = 0.001f;
	float currStoppingPower = 0.0f;
	
	// Jumping
	public float jumpForce = 8.5f;
	
	[SerializeField] float jumpCheckDistance = 1.3f;
	[SerializeField] float jumpCheckRadius = 0.7f;
	[SerializeField] LayerMask jumpLayer;
	
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

	[SerializeField] float rotCorrectionTime = 7f;
	[SerializeField] float slideTurnSpeed = 7f;
	
	public Transform model;
	Vector3 modelOffset;

	void Awake()
	{
		SetupStateMethodMap();
		ChangeState(ActorStates.Grounded);

		modelOffset = transform.position - model.position;
		currStoppingPower = stoppingSpeed;
	}

	void SetupStateMethodMap()
	{;
		stateMethodMap.Add(ActorStates.Jumping, 	Jumping);
		stateMethodMap.Add(ActorStates.Grounded, 	Grounded);
		stateMethodMap.Add(ActorStates.Rolling, 	Rolling);
	}
	
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

		CurrentStateMethod();
		ModelControl();
	}

	void Grounded()
	{
		JumpCheck();
		RollCheck();

		GroundMovement();
	}

	void Jumping()
	{
		JumpCheck();
		RollCheck();

		JumpMovement();
	}

	void Rolling()
	{
		RollCheck();
		
		MoveAtSpeed(inputVec.normalized, rollMoveSpeed);
	}

	void ChangeState(ActorStates toState)
	{
		StartCoroutine(ChangeStateRoutine(toState));
	}

	public bool CanAttack()
	{
		return !IsInState(ActorStates.Rolling);
	}

	bool IsInState(ActorStates checkState)
	{
		return currentState == checkState;
	}

	IEnumerator ChangeStateRoutine(ActorStates toState)
	{
		switch(currentState)
		{
		case ActorStates.Grounded:
			break;
		default:
			break;
		}
		
		currentState = toState;
		CurrentStateMethod = stateMethodMap[currentState];

		yield return 0;
	}

	// Do temporary state and then return to previous state
	void EnterTemporaryState(float waitTime, ActorStates tempState)
	{
		StartCoroutine(EnterTemporaryStateRoutine(waitTime, tempState, currentState));
	}

	// Do temporary state and then move on to new state
	void EnterTemporaryState(float waitTime, ActorStates tempState, ActorStates endState)
	{
		StartCoroutine(EnterTemporaryStateRoutine(waitTime, tempState, endState));
	}

	IEnumerator EnterTemporaryStateRoutine(float waitTime, ActorStates tempState, ActorStates endState)
	{
		ChangeState(tempState);

		yield return new WaitForSeconds(waitTime);

		ChangeState(endState);
	}

	void ComeToStop()
	{
		currStoppingPower -= Time.deltaTime;
		currStoppingPower = Mathf.Clamp(currStoppingPower, 0.0f, stoppingSpeed);
		
		moveVec = lastVelocity * currStoppingPower/stoppingSpeed;
		
		moveVec.y = rigidbody.velocity.y;
		rigidbody.velocity = moveVec;
		
		actor.GetAnimator().SetBool("isMoving", false);

		if(jumpColCheckTimer > jumpColCheckTime)
		{
			if(IsInState(ActorStates.Grounded))
			{
				if(stopMoveTimer >= stopMoveTime)
				{
					rigidbody.useGravity = false;
					SetFallSpeed(0f);
				}
				
				stopMoveTimer += Time.deltaTime;
			}
			else
			{
				rigidbody.useGravity = true;
			}
		}
	}
	
	void MoveAtSpeed(Vector3 inputVec, float appliedMoveSpeed)
	{
		rigidbody.useGravity = true;

		currStoppingPower = stoppingSpeed;
		
		moveVec = inputVec * appliedMoveSpeed * moveSpeedMod;
		moveVec.y = rigidbody.velocity.y;
		
		lastVelocity = moveVec;
		rigidbody.velocity = moveVec;
		
		actor.GetAnimator().SetBool("isMoving", true);
	}

	Vector3 GetInputDirection()
	{
		Vector3 inputVec = new Vector3(Input.GetAxis("Horizontal" + WadeUtils.platformName), 0.0f, Input.GetAxis("Vertical" + WadeUtils.platformName));
		if(actor.GetCamera())
		{
			inputVec = actor.GetCamera().transform.TransformDirection(inputVec);
			inputVec.y = 0f;
		}

		return inputVec;
	}

	void SetFallSpeed(float fallSpeed)
	{
		Vector3 moveVec = rigidbody.velocity;
		moveVec.y = fallSpeed;
		rigidbody.velocity = moveVec;
	}

	void GroundMovement()
	{
		inputVec = GetInputDirection();

		if(	Mathf.Abs(inputVec.magnitude) < WadeUtils.SMALLNUMBER)
		{
			ComeToStop();
		}
		else
		{
			MoveAtSpeed(inputVec, groundedMoveSpeed);
		}
	}

	void JumpMovement()
	{
		inputVec = GetInputDirection();
		
		if(	Mathf.Abs(inputVec.magnitude) < WadeUtils.SMALLNUMBER)
		{
			ComeToStop();
		}
		else
		{
			currStoppingPower = stoppingSpeed;
			
			moveVec = inputVec * jumpMoveSpeed;
			moveVec.y = rigidbody.velocity.y;
			
			lastVelocity = moveVec;
			rigidbody.velocity = moveVec;
			
			actor.GetAnimator().SetBool("isMoving", true);
		}
	}

	void RollCheck()
	{
		if(IsInState(ActorStates.Rolling))
		{
			if(rollCooldownTimer >= rollTime)
			{
				ChangeState(ActorStates.Jumping);
				actor.GetAnimator().SetBool("isRolling", false);
			}
		}
		else if(Input.GetButtonDown("Roll" + WadeUtils.platformName) && 
		        rollCooldownTimer >= rollCooldownTime && inputVec.magnitude > WadeUtils.SMALLNUMBER)
		{
			ChangeState(ActorStates.Rolling);
			actor.GetAnimator().SetBool("isRolling", true);

			rollCooldownTimer = 0f;
		}

		rollCooldownTimer += Time.deltaTime;
	}

	void AttackCheck()
	{
	}
	
	void JumpCheck()
	{
		RaycastHit hit;
		bool isOnGround = Physics.SphereCast(new Ray(transform.position, -Vector3.up), jumpCheckRadius, out hit, jumpCheckDistance, jumpLayer);

		if((isOnGround || lateJumpTimer < lateJumpTime))
		{
			if(Input.GetButtonDown("Jump" + WadeUtils.platformName))
			{
				Vector3 curVelocity = rigidbody.velocity;
				curVelocity.y = jumpForce;
				rigidbody.velocity = curVelocity;
				rigidbody.useGravity = true;

				jumpColCheckTimer = 0.0f;
				//actor.GetAnimator().SetBool("isSliding", false);
			}
			else if(jumpColCheckTimer > jumpColCheckTime && isOnGround && !IsInState(ActorStates.Rolling))
			{
				if(!IsInState(ActorStates.Grounded))
				{
					// if !launching
					ChangeState(ActorStates.Grounded);
					stopMoveTimer = 0f;
					//GainControl();
				}

				actor.GetAnimator().SetBool("isJumping", false);
				//actor.GetAnimator().SetBool("isSliding", false);
				lateJumpTimer = 0.0f;
			}
		}
		else if(!IsInState(ActorStates.Jumping) && !IsInState(ActorStates.Rolling)) // if not currently being launched
		{
			actor.GetAnimator().SetBool("isJumping", true);
			//actor.GetAnimator().SetBool("isSliding", false);
			ChangeState(ActorStates.Jumping);
		}
		
		if(!IsInState(ActorStates.Grounded))
		{
			jumpColCheckTimer += Time.deltaTime;
		}

		lateJumpTimer += Time.deltaTime;
		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
	}

	public bool IsGrabbing()
	{
		return ((WadeUtils.platformName == "_OSX" && Input.GetAxis("Grab" + WadeUtils.platformName) > WadeUtils.SMALLNUMBER) ||
		        (WadeUtils.platformName != "_OSX" && Input.GetAxis("Grab" + WadeUtils.platformName) > WadeUtils.SMALLNUMBER));
	}

	void ModelControl()
	{
		model.position = transform.position - modelOffset; // this might be important so don't delete it
		//transform.position = model.position + modelOffset;

		if(Mathf.Abs(Input.GetAxisRaw("Horizontal" + WadeUtils.platformName)) > WadeUtils.SMALLNUMBER || Mathf.Abs(Input.GetAxisRaw("Vertical" + WadeUtils.platformName)) > WadeUtils.SMALLNUMBER)
		{
			Vector3 lookVec = moveVec;
			lookVec.y = 0.0f;
			
			if(lookVec != Vector3.zero)
			{
				model.rotation = Quaternion.LookRotation(lookVec * 10.0f, transform.up);
			}
		}
	}

	void SlideModelControl()
	{
		model.position = transform.position - modelOffset; // this might be important so don't delete it

		Vector3 lookVec = rigidbody.velocity.normalized;
		lookVec.y = 0.0f;
		
		if(lookVec != Vector3.zero)
		{
			model.rotation = Quaternion.Lerp(model.rotation, 
			                                 Quaternion.LookRotation(lookVec * 10.0f, transform.up),
			                                 Time.deltaTime * slideTurnSpeed);
		}
	}

	public void Stop()
	{
		rigidbody.velocity = Vector3.zero;
	}

	void OnLandFromLaunch()
	{
		ChangeState(ActorStates.Grounded);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, 0.5f);
		Gizmos.DrawWireSphere(transform.position - Vector3.up * 0.5f, 0.5f);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position - Vector3.up, jumpCheckRadius);
	}
}
