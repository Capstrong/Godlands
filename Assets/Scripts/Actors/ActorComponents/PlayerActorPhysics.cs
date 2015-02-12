using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerActor ) )]
public class PlayerActorPhysics : ActorPhysics
{
	PlayerActor _actor;
	bool _jumpButtonDown = false;
	bool _isGrabbing = false;

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

		_isGrabbing = WadeUtils.ValidAxisInput("Grab");
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

		if ( _isGrabbing )
		{
			ClimbCheck();
		}

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
		if ( ClimbCheck() && _isGrabbing && ( actor as PlayerActor ).actorStats.CanUseStat( Stat.Stamina ) )
		{
			( actor as PlayerActor ).actorStats.StartUsingStat( Stat.Stamina );
			ClimbSurface( GetInput() );
		}
		else
		{
			( actor as PlayerActor ).actorStats.StopUsingStat( Stat.Stamina );
			StopClimbing();
		}
	}

	void Grounded()
	{
		if ( _jumpButtonDown )
		{
			JumpCheck();
		}

		if ( _isGrabbing )
		{
			ClimbCheck();
		}

		RollCheck();

		GroundMovement();
	}

	void GroundMovement()
	{
		inputVec = GetMoveDirection();

		if ( inputVec.IsZero() )
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
		inputVec = GetMoveDirection();

		if ( inputVec.IsZero() )
		{
			ComeToStop();
		}
		else
		{
			currStoppingPower = stoppingSpeed;

			moveVec = inputVec * jumpMoveSpeed;
			moveVec.y = rigidbody.velocity.y;

			rigidbody.velocity = moveVec;

			if ( _actor.animator )
			{
				_actor.animator.SetBool( "isMoving", true );
			}
		}
	}

	Vector3 GetInput()
	{
		return new Vector3( Input.GetAxis( "Horizontal" + WadeUtils.platformName ),
		                    0.0f,
		                    Input.GetAxis( "Vertical" + WadeUtils.platformName ) );
	}

	/**
	 * Calculates the direction of movement based on the input and the camera position.
	 */
	Vector3 GetMoveDirection()
	{
		Vector3 inputVec = GetInput();

		if ( WadeUtils.IsNotZero( inputVec.x ) && WadeUtils.IsNotZero( inputVec.z ) )
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
