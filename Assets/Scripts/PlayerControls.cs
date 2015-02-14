using UnityEngine;
using System.Collections;

[RequireComponent( typeof( ActorPhysics ) )]
public class PlayerControls : MonoBehaviour
{
	PlayerActor _actor;
	ActorPhysics _actorPhysics;
	bool _jumpButtonDown = false;
	bool _isGrabbing = false;

	void Awake()
	{
		_actor = GetComponent<PlayerActor>();
		_actorPhysics = GetComponent<ActorPhysics>();

		SetupStateMethodMap();
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

	void SetupStateMethodMap()
	{
		_actorPhysics.RegisterStateMethod( ActorStates.Jumping, Jumping );
		_actorPhysics.RegisterStateMethod( ActorStates.Falling, Jumping );
		_actorPhysics.RegisterStateMethod( ActorStates.Grounded, Grounded );
		_actorPhysics.RegisterStateMethod( ActorStates.Rolling, Rolling );
		_actorPhysics.RegisterStateMethod( ActorStates.Climbing, Climbing );
	}

	void Jumping()
	{
		if ( _jumpButtonDown )
		{
			_actorPhysics.JumpCheck();
		}

		if ( _isGrabbing )
		{
			_actorPhysics.ClimbCheck();
		}

		_actorPhysics.RollCheck();

		_actorPhysics.JumpMovement( GetMoveDirection() );
	}

	void Rolling()
	{
		_actorPhysics.RollCheck();
		_actorPhysics.RollMovement();
	}

	void Climbing()
	{
		if ( _actorPhysics.ClimbCheck() &&
			_isGrabbing &&
			_actor.actorStats.CanUseStat( Stat.Stamina ) )
		{
			_actor.actorStats.StartUsingStat( Stat.Stamina );
			_actorPhysics.ClimbSurface( GetInput() );
		}
		else
		{
			_actor.actorStats.StopUsingStat( Stat.Stamina );
			_actorPhysics.StopClimbing();
		}
	}

	void Grounded()
	{
		if ( _jumpButtonDown )
		{
			_actorPhysics.JumpCheck();
		}

		if ( _isGrabbing )
		{
			_actorPhysics.ClimbCheck();
		}

		_actorPhysics.RollCheck();

		GroundMovement();
	}

	void GroundMovement()
	{
		Vector3 inputVec = GetMoveDirection();

		if ( inputVec.IsZero() )
		{
			_actorPhysics.ComeToStop();
		}
		else
		{
			_actorPhysics.GroundMovement( inputVec );
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
