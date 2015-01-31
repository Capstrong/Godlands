using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerActor ) )]
[RequireComponent( typeof( PlayerActorStats ) )]
public class PlayerActorPhysics : ActorPhysics
{
	PlayerActor _actor;
	PlayerActorStats _actorStats;

	public override void Awake()
	{
		base.Awake();

		_actor = GetComponent<PlayerActor>();
		_actorStats = GetComponent<PlayerActorStats>();

		SetupStateMethodMap();
		ChangeState( ActorStates.Grounded );
	}

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.Escape ) )
		{
			Application.Quit();
		}

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

		if ( Input.GetKeyDown(KeyCode.J) )
		{
			_actorStats.StartUsingStamina();
		}

		if (Input.GetKeyUp(KeyCode.J))
		{
			_actorStats.StopUsingStamina();
		}

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
		if ( _actor.actorCamera.cam )
		{
			inputVec = _actor.actorCamera.cam.transform.TransformDirection( inputVec );
			inputVec.y = 0f;
		}

		return inputVec;
	}
}
