using UnityEngine;
using System.Collections;

[RequireComponent( typeof( ActorPhysics ), typeof( PlayerActor ) )]
public class PlayerControls : MonoBehaviour
{
	[Tooltip( "The distance forward from the camera's position to check for objects that can be interacted with." )]
	[SerializeField] float _interactCheckDistance = 5.0f;
	[SerializeField] float _interactCheckRadius = 0.2f;
	[SerializeField] Vector3 _interactRaycastOrigin = Vector3.zero;

	[Tooltip("Maximum length of time in seconds that the physics will push the player up while holding the jump button")]
	[SerializeField] float _maxJumpForceTime = 0f;
	public float maxJumpForceTime
	{
		get { return _maxJumpForceTime; }
	}

	PlayerActor _actor;

	[Tooltip( "Probably straight up" )]
	[SerializeField] Vector3 _respawnOffset = Vector3.zero;
	[ReadOnly( "Respawn Position" )]
	[SerializeField] Vector3 _respawnPosition = Vector3.zero;
	Quaternion _respawnRotation = Quaternion.identity;

	[SerializeField] AudioSource _respawnSound = null;
	
	Transform _cameraTransform = null;
	
	TextBox _textBox = null;
	
	[SerializeField] LayerMask _interactableLayer = 0;
	public LayerMask interactableLayer
	{
		get { return _interactableLayer; }
	}

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

	Button _sprintButton = new Button( "Sprint" );
	public Button sprintButton
	{
		get { return _sprintButton; }
	}

	void Awake()
	{
		_actor = GetComponent<PlayerActor>();
		_cameraTransform = Camera.main.transform;

		SetupStateMethodMap();

		_respawnPosition = transform.position + _respawnOffset;
		_respawnRotation = transform.rotation;

		DayCycleManager.RegisterEndOfDayCallback( Respawn );
		_textBox = FindObjectOfType<TextBox>();
	}

	void FixedUpdate()
	{
		if ( Input.GetKeyDown( KeyCode.Escape ) )
		{
			Application.Quit();
		}

		if ( Input.GetKeyDown( KeyCode.Y ) )
		{
			Respawn();
		}

		_holdButton.Update();
		_useButton.Update();
		_jumpButton.Update();
		_sprintButton.Update();
	}

	#region Physics States
	public class Jumping : PhysicsState
	{
		public PlayerActor player;

		bool _forceUp = true; // Stays true until the player releases the button or the timer runs out
		float _jumpTimer = 0f;

		public Jumping( PlayerActor player )
		{
			this.player = player;
		}

		public override void Enter()
		{
			player.animator.SetBool( "isJumping", true );
			player.physics.DoJump();
			_jumpTimer = 0f;
			_forceUp = true;
		}

		public override void Update()
		{
			_jumpTimer += Time.deltaTime;

			// Whether to continue forcing upwards with constant velocity
			if ( _forceUp && ( !player.controls.jumpButton || _jumpTimer > player.controls.maxJumpForceTime ) )
			{
				_forceUp = false;
			}

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
				     player.stats.CanUseStat( Stat.Stamina ) &&
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
					player.physics.AirMovement( player.controls.GetMoveDirection(), _forceUp );
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
				     player.stats.CanUseStat( Stat.Stamina ) &&
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
			if ( player.controls.holdButton &&
			     player.stats.CanUseStat( Stat.Stamina ) &&
			     player.physics.ClimbCheck() )
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
			player.physics.StartLateJumpTimer();
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
				else if ( player.controls.holdButton &&
				          player.stats.CanUseStat( Stat.Stamina ) &&
				          player.physics.ClimbCheck() )
				{
					player.physics.ChangeState( PhysicsStateType.Climbing );
				}
				else if ( player.controls.sprintButton )
				{
					player.physics.ChangeState( PhysicsStateType.Sprinting );
				}
				else if ( player.controls.useButton.down )
				{
					if ( player.inventory.CanUseItemWithoutTarget() )
					{
						player.inventory.UseItem();
					}
					else
					{
						Debug.Log( "Checking raycast forward." );

						// This allows us to do one raycast for all actions
						// which is good since we do RaycastAll(), which is expensive.
						RaycastHit hitInfo;
						if ( player.controls.RaycastForward( out hitInfo ) )
						{
							if ( player.cutting.CutCheck( hitInfo ) )
							{
								// cutting was done
							}
							else if ( player.inventory.UseItemWithTarget( hitInfo ) )
							{
								// item use / buddy spawning was done
							}
							else if ( player.controls.AdultBuddyCheck( hitInfo ) )
							{
								Debug.Log( "Made buddy an adult" );
								// Interaction was done.
							}
							else if ( player.controls.InteractCheck( hitInfo ) )
							{
								// Interaction was done
							}
						}
					}
				}

				player.physics.GroundMovement( player.controls.GetMoveDirection() );

				player.animator.SetFloat( "moveSpeed", player.physics.normalizedMoveSpeed );
			}
			else
			{
				player.physics.ChangeState( PhysicsStateType.Falling );
				player.physics.StartLateJumpTimer();
			}
		}

		public override void Exit() { }
	}

	public class Sprinting : PhysicsState
	{
		PlayerActor player;

		public Sprinting( PlayerActor player )
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

				if ( !player.controls.sprintButton )
				{
					player.physics.ChangeState( PhysicsStateType.Grounded );
				}

				player.physics.GroundMovement( player.controls.GetMoveDirection(), true );

				player.animator.SetFloat( "moveSpeed", player.physics.normalizedMoveSpeed );
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
			else if ( player.physics.GroundedCheck() )
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
			else if ( player.controls.holdButton &&
			          player.stats.CanUseStat( Stat.Gliding ) )
			{
				player.physics.GlideMovement( player.controls.GetMoveDirection() );
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
		_actor.physics.RegisterState( PhysicsStateType.Jumping,   new Jumping( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Falling,   new Falling( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Grounded,  new Grounded( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Climbing,  new Climbing( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Gliding,   new Gliding( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Sprinting, new Sprinting( _actor ) );
	}
	#endregion

	public bool InteractCheck( RaycastHit hitInfo )
	{
		Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
		if ( interactable )
		{
			_textBox.SetTextForDuration( interactable.interactText, interactable.duration );
			return true;
		}

		return false;
	}

	public bool AdultBuddyCheck( RaycastHit hitInfo )
	{
		Debug.Log( "Checking if we can make buddy an adult." );
		BuddyStats buddyStats = hitInfo.collider.GetComponentInParent<BuddyStats>();
		if ( buddyStats && buddyStats.isAdult )
		{
			// TODO: How do we start the process of actually making the buddy an adult?
			buddyStats.gameObject.SetActive( false );
			AdultManager.SpawnAdult();
		}

		return false;
	}

	/**
	 * Raycast forward from the camera's position to detect
	 * items to interact with.
	 * 
	 * This performs RaycastAll, and returns the closest collision.
	 */
	bool RaycastForward( out RaycastHit closestHit )
	{
		Vector3 rayOrigin = transform.position + _interactRaycastOrigin;
		Vector3 camForward = _cameraTransform.forward;
	
		Debug.DrawRay( rayOrigin, camForward * _interactCheckDistance, Color.yellow, 1.0f, false );

		RaycastHit[] hits = Physics.SphereCastAll(
			new Ray( rayOrigin, camForward ),
			_interactCheckRadius,
			_interactCheckDistance,
			_actor.cutting.cuttableLayer | _actor.inventory.buddyLayer | _interactableLayer );

		if ( hits.Length == 0 )
		{
			closestHit = new RaycastHit();
			return false;
		}

		closestHit = hits[0];
		float closestDistance = ( rayOrigin - closestHit.point ).sqrMagnitude;
		foreach ( RaycastHit hit in hits )
		{
			float hitDistance = ( rayOrigin - hit.point ).sqrMagnitude;
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
		Teleport( _respawnPosition, _respawnRotation );
		SoundManager.Play2DSound( _respawnSound );
	}

	public void Teleport( Vector3 toPosition, Quaternion toRotation = new Quaternion(), bool snapCamera = true )
	{
		transform.position = toPosition;
		transform.rotation = toRotation;

		if ( snapCamera )
		{
			_cameraTransform.position = toPosition;
		}

		_actor.physics.ChangeState( PhysicsStateType.Falling );
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere( GetComponent<Transform>().position + _interactRaycastOrigin,
		                   0.1f );
	}
}
