using UnityEngine;
using System.Collections;

[RequireComponent( typeof( ActorPhysics ) )]
public class PlayerControls : MonoBehaviour
{
	[Tooltip( "The distance forward from the camera's position to check for objects that can be interacted with." )]
	[SerializeField] float _interactCheckDistance = 5.0f;

	PlayerActor _actor;
	ActorPhysics _actorPhysics;
	Cutting _cutting;
	bool _holdButtonDown = false;
	bool _useButtonLast = false;
	bool _useButtonDown = false;
	bool _useButtonPressed = false;
	bool _jumpButtonDown = false;

	void Awake()
	{
		_actor = GetComponent<PlayerActor>();
		_actorPhysics = GetComponent<ActorPhysics>();
		_cutting = GetComponent<Cutting>();

		SetupStateMethodMap();
	}

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.Escape ) )
		{
			Application.Quit();
		}

		_holdButtonDown = InputUtils.GetButton( "Hold" );

		_useButtonLast = _useButtonDown;
		_useButtonDown  = InputUtils.GetButton( "Use" );
		_useButtonPressed = _useButtonDown && !_useButtonLast;

		_jumpButtonDown = InputUtils.GetButton( "Jump" );
	}

	#region Physics States
	void SetupStateMethodMap()
	{
		_actorPhysics.RegisterStateMethod( ActorStates.Jumping,  Jumping );
		_actorPhysics.RegisterStateMethod( ActorStates.Falling,  Jumping );
		_actorPhysics.RegisterStateMethod( ActorStates.Grounded, Grounded );
		_actorPhysics.RegisterStateMethod( ActorStates.Rolling,  Rolling );
		_actorPhysics.RegisterStateMethod( ActorStates.Climbing, Climbing );
		_actorPhysics.RegisterStateMethod( ActorStates.Gliding,  Gliding );
	}

	void Jumping()
	{
		if ( _actorPhysics.GroundedCheck() && _jumpButtonDown )
		{
			_actorPhysics.JumpCheck();
		}
		else
		{
			if ( _holdButtonDown )
			{
				_actorPhysics.ClimbCheck();
			}

			_actorPhysics.RollCheck();

			if ( _holdButtonDown && _actor.actorStats.CanUseStat( Stat.Gliding ) )
			{
				_actor.actorStats.StartUsingStat( Stat.Gliding );
				_actorPhysics.StartGlide();
			}
			else
			{
				_actorPhysics.JumpMovement( GetMoveDirection() );
			}
		}
	}

	void Rolling()
	{
		_actorPhysics.RollCheck();
		_actorPhysics.RollMovement();
	}

	void Climbing()
	{
		if ( _actorPhysics.ClimbCheck() &&
		     _holdButtonDown &&
		     _actor.actorStats.CanUseStat( Stat.Stamina ) )
		{
			_actor.actorStats.StartUsingStat( Stat.Stamina );
			_actorPhysics.ClimbSurface( GetMoveInput() );
		}
		else
		{
			_actor.actorStats.StopUsingStat( Stat.Stamina );
			_actorPhysics.StopClimbing();
		}
	}

	void Grounded()
	{
		if ( _actorPhysics.GroundedCheck() )
		{
			if ( _jumpButtonDown )
			{
				_actorPhysics.JumpCheck();
			}

			if ( _holdButtonDown )
			{
				_actorPhysics.ClimbCheck();
			}

			if ( _useButtonPressed )
			{
				// This allows us to do one raycast for both actions
				// which is good since we do RaycastAll(), which is expensive.
				RaycastHit hitInfo;
				if ( RaycastForward( out hitInfo ) )
				{
					if ( !_cutting.CutCheck( hitInfo ) )
					{
						_actor.actorResources.CheckUseItem( hitInfo );
					}
				}
			}

			_actorPhysics.RollCheck();

			GroundMovement();
		}
	}

	void Gliding()
	{
		if ( _jumpButtonDown )
		{
			_actorPhysics.JumpCheck();
		}

		if ( _holdButtonDown )
		{
			_actorPhysics.ClimbCheck();
		}

		if ( !_actorPhysics.GroundedCheck() &&
		     _holdButtonDown &&
		     _actor.actorStats.CanUseStat( Stat.Gliding ) )
		{
			_actorPhysics.GlideMovement( GetMoveDirection() );
		}
		else
		{
			_actor.actorStats.StopUsingStat( Stat.Gliding );
			_actorPhysics.EndGlide();
		}
	}
	#endregion

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

	/**
	 * Raycast forward from the camera's position to detect
	 * items to interact with.
	 * 
	 * This performs RaycastAll, and returns the closest collision.
	 */
	bool RaycastForward( out RaycastHit closestHit )
	{
		Vector3 camPos = Camera.main.transform.position;
		Vector3 camForward = Camera.main.transform.forward;

		RaycastHit[] hits = Physics.RaycastAll(
			new Ray( transform.position, camForward ),
			_interactCheckDistance,
			_cutting.cuttableLayer | _actor.actorResources.buddyLayer );

		if ( hits.Length == 0 )
		{
			closestHit = new RaycastHit();
			return false;
		}

		closestHit = hits[0];
		float closestDistance = ( camPos - closestHit.point ).sqrMagnitude;
		foreach ( RaycastHit hit in hits )
		{
			float hitDistance = ( camPos - hit.point ).sqrMagnitude;
			if ( hitDistance < closestDistance )
			{
				closestHit = hit;
				closestDistance = hitDistance;
			}
		}

		return true;
	}

	/**
	 * Retrives the raw input values, with horizontal mapped to X
	 * and vertical mapped to Z.
	 */
	Vector3 GetMoveInput()
	{
		return new Vector3( Input.GetAxis( "Horizontal" + PlatformUtils.platformName ),
		                    0.0f,
		                    Input.GetAxis( "Vertical" + PlatformUtils.platformName ) );
	}

	/**
	 * Calculates the direction of movement based on the input and the camera position.
	 */
	Vector3 GetMoveDirection()
	{
		Vector3 inputVec = GetMoveInput();

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
