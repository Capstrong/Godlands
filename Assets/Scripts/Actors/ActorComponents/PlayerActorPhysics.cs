using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerActor ) )]
public class PlayerActorPhysics : ActorPhysics
{
	PlayerActor _actor;

	public override void Awake()
	{
		base.Awake();

		_actor = GetComponent<PlayerActor>();

		SetupStateMethodMap();
		ChangeState( ActorStates.Grounded );
	}

	void Update()
	{
		if( Input.GetKeyDown( KeyCode.Escape ) )
		{
			Application.Quit();
		}

		CurrentStateMethod();
		ModelControl();
	}

	void SetupStateMethodMap()
	{
		stateMethodMap.Add( ActorStates.Jumping,  Jumping );
		stateMethodMap.Add( ActorStates.Grounded, Grounded );
		stateMethodMap.Add( ActorStates.Rolling,  Rolling );
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
		
		MoveAtSpeed( inputVec.normalized, rollMoveSpeed );
	}

	void Grounded()
	{
		JumpCheck();
		RollCheck();

		GroundMovement();
	}

	void GroundMovement()
	{
		inputVec = GetInputDirection();

		if ( Mathf.Abs( inputVec.magnitude ) < WadeUtils.SMALLNUMBER )
		{
			ComeToStop();
		}
		else
		{
			MoveAtSpeed( inputVec, groundedMoveSpeed );
		}
	}

	void JumpMovement()
	{
		inputVec = GetInputDirection();
		
		if( Mathf.Abs( inputVec.magnitude ) < WadeUtils.SMALLNUMBER )
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
			
			if ( actor.GetAnimator() )
			{
				actor.GetAnimator().SetBool( "isMoving", true );
			}
		}
	}

	void ModelControl()
	{
		model.position = transform.position - modelOffset; // this might be important so don't delete it
		//transform.position = model.position + modelOffset;

		if ( Mathf.Abs( Input.GetAxisRaw( "Horizontal" + WadeUtils.platformName ) ) > WadeUtils.SMALLNUMBER ||
		     Mathf.Abs( Input.GetAxisRaw( "Vertical" + WadeUtils.platformName ) ) > WadeUtils.SMALLNUMBER )
		{
			Vector3 lookVec = moveVec;
			lookVec.y = 0.0f;
			
			if(lookVec != Vector3.zero)
			{
				model.rotation = Quaternion.LookRotation(lookVec * 10.0f, transform.up);
			}
		}
	}

	Vector3 GetInputDirection()
	{
		Vector3 inputVec = new Vector3(
			Input.GetAxis( "Horizontal" + WadeUtils.platformName ),
		                   0.0f,
		                   Input.GetAxis( "Vertical" + WadeUtils.platformName ) );
		if ( _actor.actorCamera.cam != null )
		{
			inputVec = _actor.actorCamera.cam.transform.TransformDirection( inputVec );
			inputVec.y = 0f;
		}

		return inputVec;
	}
}
