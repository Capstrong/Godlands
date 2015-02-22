using UnityEngine;
using System.Collections;

[RequireComponent( typeof( ActorPhysics ) )]
public class PlayerControls : MonoBehaviour
{
	[Tooltip( "The distance forward from the camera's position to check for objects that can be interacted with." )]
	[SerializeField] float _interactCheckDistance = 5.0f;
	[SerializeField] float _interactCheckRadius = 0.2f;

	PlayerActor _actor;
	
	Button _holdButton = new Button( "Hold" );
	public Button holdButton
	{
		get { return _holdButton; }
	}

	Button _useButton  = new Button( "Use" );
	public Button useButton
	{
		get { return _useButton; }
	}

	Button _jumpButton = new Button( "Jump" );
	public Button jumpButton
	{
		get { return _jumpButton; }
	}

	void Awake()
	{
		_actor = GetComponent<PlayerActor>();

		SetupStateMethodMap();
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
	public class Jumping : PhysicsState
	{
		public PlayerActor player;

		public Jumping( PlayerActor player )
		{
			this.player = player;
		}

		public override void Enter()
		{
			player.animator.SetBool( "isJumping", true );
			player.physics.DoJump();
		}

		public override void Update()
		{
			if ( player.physics.GroundedCheck() )
			{
				if ( player.controls.jumpButton.down &&
				     player.physics.JumpCheck() )
				{
					player.physics.ChangeState( PhysicsStateType.Jumping );
				}
				else
				{
					player.physics.ChangeState( PhysicsStateType.Grounded );
				}
			}
			else
			{
				if ( player.controls.holdButton &&
				     player.physics.ClimbCheck() )
				{
					player.physics.ChangeState( PhysicsStateType.Climbing );
				}

				if ( player.controls.holdButton.down && player.stats.CanUseStat( Stat.Gliding ) )
				{
					player.physics.ChangeState( PhysicsStateType.Gliding );
				}
				else
				{
					player.physics.AirMovement( player.controls.GetMoveDirection() );
				}
			}
		}

		public override void Exit()
		{
			player.animator.SetBool( "isJumping", false );
		}
	}

	public class Falling : PhysicsState
	{
		public PlayerActor player;

		public Falling( PlayerActor player )
		{
			this.player = player;
		}

		public override void Enter()
		{
			player.animator.SetBool( "isJumping", true );
			player.physics.StartLateJumpTimer();
		}

		public override void Update()
		{
			if ( player.physics.GroundedCheck() )
			{
				player.physics.ChangeState( PhysicsStateType.Grounded );
			}
			else if ( player.controls.jumpButton.down &&
			          player.physics.JumpCheck() )
			{
				player.physics.ChangeState( PhysicsStateType.Jumping );
			}
			else
			{
				if ( player.controls.holdButton &&
				     player.physics.ClimbCheck() )
				{
					player.physics.ChangeState( PhysicsStateType.Climbing );
				}

				if ( player.controls.holdButton.down && player.stats.CanUseStat( Stat.Gliding ) )
				{
					player.physics.ChangeState( PhysicsStateType.Gliding );
				}
				else
				{
					player.physics.AirMovement( player.controls.GetMoveDirection() );
				}
			}
		}

		public override void Exit()
		{
			player.animator.SetBool( "isJumping", false );
		}
	}

	public class Climbing : PhysicsState
	{
		PlayerActor player;

		public Climbing( PlayerActor player )
		{
			this.player = player;
		}

		public override void Enter()
		{
			player.animator.SetBool( "isClimbing", true );
			player.stats.StartUsingStat( Stat.Stamina );
			player.physics.StartClimbing();
		}

		public override void Update()
		{
			if ( player.physics.ClimbCheck() &&
			     player.controls.holdButton &&
			     player.stats.CanUseStat( Stat.Stamina ) )
			{
				player.physics.ClimbSurface( player.controls.GetMoveInput() );
			}
			else
			{
				player.physics.ChangeState( PhysicsStateType.Falling );
			}
		}

		public override void Exit()
		{
			player.animator.SetBool( "isClimbing", false );
			player.stats.StopUsingStat( Stat.Stamina );
			player.physics.StopClimbing();
		}
	}

	public class Grounded : PhysicsState
	{
		PlayerActor player;

		public Grounded( PlayerActor player )
		{
			this.player = player;
		}

		public override void Enter() { }

		public override void Update()
		{
			if ( player.physics.GroundedCheck() )
			{
				if ( player.controls.jumpButton.down &&
				     player.physics.JumpCheck() )
				{
					player.physics.ChangeState( PhysicsStateType.Jumping );
				}

				if ( player.controls.holdButton &&
				     player.physics.ClimbCheck() )
				{
					player.physics.ChangeState( PhysicsStateType.Climbing );
				}

				if ( player.controls.useButton.down )
				{
					if ( player.inventory.CanUseItemWithoutTarget() )
					{
						player.inventory.UseItem();
					}
					else
					{
						// This allows us to do one raycast for both actions
						// which is good since we do RaycastAll(), which is expensive.
						RaycastHit hitInfo;
						if ( player.controls.RaycastForward( out hitInfo ) )
						{
							if ( !player.cutting.CutCheck( hitInfo ) )
							{
								player.inventory.UseItemWithTarget( hitInfo );
							}
						}
					}
				}

				player.controls.GroundMovement();

				player.animator.SetFloat( "moveSpeed", player.rigidbody.velocity.magnitude );
			}
			else
			{
				player.physics.ChangeState( PhysicsStateType.Falling );
			}
		}

		public override void Exit() { }
	}

	public class Gliding : PhysicsState
	{
		PlayerActor player;

		public Gliding( PlayerActor player )
		{
			this.player = player;
		}

		public override void Enter()
		{
			player.physics.StartGliding();
			player.stats.StartUsingStat( Stat.Gliding );
			player.animator.SetBool( "isGliding", true );
		}

		public override void Update()
		{
			if ( player.controls.holdButton &&
			     player.physics.ClimbCheck() )
			{
				player.physics.ChangeState( PhysicsStateType.Climbing );
			}
			else if ( player.controls.holdButton &&
			          player.stats.CanUseStat( Stat.Gliding ) )
			{
				if ( !player.physics.GroundedCheck() )
				{
					player.physics.GlideMovement( player.controls.GetMoveDirection() );
				}
				else
				{
					if ( player.controls.jumpButton.down &&
					     player.physics.JumpCheck() )
					{
						player.physics.ChangeState( PhysicsStateType.Jumping );
					}
					else
					{
						player.physics.ChangeState( PhysicsStateType.Grounded );
					}
				}
			}
			else
			{
				player.physics.ChangeState( PhysicsStateType.Falling );
			}
		}

		public override void Exit()
		{
			player.stats.StopUsingStat( Stat.Gliding );
			player.animator.SetBool( "isGliding", false );
			player.physics.StopGliding();
		}
	}

	void SetupStateMethodMap()
	{
		_actor.physics.RegisterState( PhysicsStateType.Jumping,  new Jumping( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Falling,  new Falling( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Grounded, new Grounded( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Climbing, new Climbing( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Gliding,  new Gliding( _actor ) );
	}
	#endregion

	void GroundMovement()
	{
		Vector3 inputVec = GetMoveDirection();

		if ( inputVec.IsZero() )
		{
			_actor.physics.ComeToStop();
		}
		else
		{
			_actor.physics.GroundMovement( inputVec );
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
			_actor.cutting.cuttableLayer | _actor.inventory.buddyLayer );

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
}
