using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerActor ) )]
public class PlayerActorPhysics : ActorPhysics
{
	PlayerActor _actor;
	bool _jumpButtonDown = false;

	public override void Awake()
	{
		base.Awake();

		_actor = GetComponent<PlayerActor>();
		ChangeState( ActorStates.Grounded );
	}

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.Escape ) )
		{
			Application.Quit();
		}

		_jumpButtonDown = Input.GetButtonDown( "Jump" + WadeUtils.platformName );
	}

	public override void SetupStateMethodMap()
	{
		stateMethodMap.Add( ActorStates.Jumping, Jumping );
		stateMethodMap.Add( ActorStates.Falling, Jumping );
		stateMethodMap.Add( ActorStates.Grounded, Grounded );
		stateMethodMap.Add( ActorStates.Rolling, Rolling );
		stateMethodMap.Add( ActorStates.Climbing, Climbing );
	}

	void Jumping()
	{
		if ( _jumpButtonDown )
		{
			JumpCheck();
		}
		ClimbCheck();
		RollCheck();

		JumpMovement();
	}

	void Rolling()
	{
		RollCheck();

		MoveAtSpeed( inputVec.normalized, rollMoveSpeed );
	}

	void Climbing()
	{
		inputVec = GetInputDirection();

		ClimbSurface();
		ClimbCheck();

		if ( !isGrabbing )
		{
			StopClimbing();
		}
	}

	void Grounded()
	{
		if ( _jumpButtonDown )
		{
			JumpCheck();
		}
		ClimbCheck();
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

			moveVec = inputVec * jumpMoveSpeed;
			moveVec.y = rigidbody.velocity.y;

			lastVelocity = moveVec;
			rigidbody.velocity = moveVec;

			if ( _actor.animator )
			{
				_actor.animator.SetBool( "isMoving", true );
			}
		}
	}

	Vector3 GetInputDirection()
	{
		Vector3 inputVec = new Vector3( Input.GetAxis( "Horizontal" + WadeUtils.platformName ),
		                                0.0f,
		                                Input.GetAxis( "Vertical" + WadeUtils.platformName ) );

		if ( WadeUtils.IsNotZero( inputVec.x ) && WadeUtils.IsNotZero( inputVec.z ))
		{
			inputVec *= WadeUtils.DUALINPUTMOD; // this reduces speed of diagonal movement
		}

		if ( _actor.actorCamera.cam )
		{
			inputVec = _actor.actorCamera.cam.transform.TransformDirection( inputVec );
			inputVec.y = 0f;
		}

		return inputVec;
	}
}
