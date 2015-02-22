using UnityEngine;
using System.Collections;

[RequireComponent( typeof( ActorPhysics ) )]
public class PlayerControls : MonoBehaviour
{
	[Tooltip( "The distance forward from the camera's position to check for objects that can be interacted with." )]
	[SerializeField] float _interactCheckDistance = 5.0f;
	[SerializeField] float _interactCheckRadius = 0.2f;

	PlayerActor _actor;
	ActorPhysics _actorPhysics;
	Cutting _cutting;
	
	Button _holdButton = new Button( "Hold" );
	Button _useButton  = new Button( "Use" );
	Button _jumpButton = new Button( "Jump" );

	Vector3 _respawnPosition = new Vector3();
	Quaternion _respawnRotation = new Quaternion();
	[Tooltip("Probably a straight up vertical offset")]
	[SerializeField] Vector3 _respawnOffset = new Vector3();

	void Awake()
	{
		_actor = GetComponent<PlayerActor>();
		_actorPhysics = GetComponent<ActorPhysics>();
		_cutting = GetComponent<Cutting>();

		SetupStateMethodMap();

		_respawnPosition = transform.position + _respawnOffset;
		_respawnRotation = transform.rotation;
	}

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.Escape ) )
		{
			Application.Quit();
		}

		_holdButton.Update();
		_useButton.Update();
		_jumpButton.Update();
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
		if ( _actorPhysics.GroundedCheck() && _jumpButton.down )
		{
			_actorPhysics.JumpCheck();
		}
		else
		{
			if ( _holdButton )
			{
				_actorPhysics.ClimbCheck();
			}

			_actorPhysics.RollCheck();

			if ( _holdButton && _actor.actorStats.CanUseStat( Stat.Gliding ) )
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
		     _holdButton &&
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
			if ( _jumpButton.down )
			{
				_actorPhysics.JumpCheck();
			}

			if ( _holdButton )
			{
				_actorPhysics.ClimbCheck();
			}

			if ( _useButton.down )
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
		if ( _jumpButton.down )
		{
			_actorPhysics.JumpCheck();
		}

		if ( _holdButton )
		{
			_actorPhysics.ClimbCheck();
		}

		if ( !_actorPhysics.GroundedCheck() &&
		     _holdButton &&
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

		Debug.DrawRay( camPos, camForward * _interactCheckDistance, Color.yellow, 1.0f, false );

		RaycastHit[] hits = Physics.SphereCastAll(
			new Ray( camPos, camForward ),
			_interactCheckRadius,
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
		return new Vector3( InputUtils.GetAxis( "Horizontal" ),
		                    0.0f,
		                    InputUtils.GetAxis( "Vertical" ) );
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

	public void Respawn()
	{
		_actorPhysics.transform.position = _respawnPosition;
		_actorPhysics.transform.rotation = _respawnRotation;
		_actorPhysics.ChangeState( ActorStates.Falling );
	}
}
