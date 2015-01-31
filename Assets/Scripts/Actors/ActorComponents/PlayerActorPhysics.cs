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

	void FixedUpdate()
	{
		if ( Input.GetKeyDown( KeyCode.Escape ) )
		{
			Application.Quit();
		}

		rigVelocity = rigidbody.velocity;

		CurrentStateMethod();
		ModelControl();
	}

	void SetupStateMethodMap()
	{
		stateMethodMap.Add( ActorStates.Jumping, Jumping );
		stateMethodMap.Add( ActorStates.Grounded, Grounded );
		stateMethodMap.Add( ActorStates.Rolling, Rolling );
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

		if ( Mathf.Abs( inputVec.magnitude ) < WadeUtils.SMALLNUMBER )
		{
			ComeToStop();
		}
		else
		{
			currStoppingPower = stoppingSpeed;

			CheckGroundSlope();

			moveVec = inputVec * jumpMoveSpeed * groundSlopeSpeedMod;
			moveVec.y = rigidbody.velocity.y;

			lastVelocity = moveVec;
			rigidbody.velocity = moveVec;

			if ( actor.animator )
			{
				actor.animator.SetBool( "isMoving", true );
			}
		}
	}

	Vector3 GetInputDirection()
	{
		Vector3 inputVec = new Vector3( Input.GetAxis( "Horizontal" + WadeUtils.platformName ),
		                                0.0f,
		                                Input.GetAxis( "Vertical" + WadeUtils.platformName ) );

		if( Mathf.Abs( inputVec.x ) > WadeUtils.SMALLNUMBER && Mathf.Abs( inputVec.z ) > WadeUtils.SMALLNUMBER )
		{
			inputVec *= dualInputMod; // this reduces speed of diagonal movement
		}

		if ( _actor.actorCamera.cam )
		{
			inputVec = _actor.actorCamera.cam.transform.TransformDirection( inputVec );
			inputVec.y = 0f;
		}

		return inputVec;
	}
}
