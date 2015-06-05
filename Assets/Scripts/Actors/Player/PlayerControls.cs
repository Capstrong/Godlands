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

	public float _defaultCameraZoom = 10.0f;
	public float _glidingCameraZoom = 15.0f;
	public float _sprintingCameraZoom = 15.0f;
	public float _climbingCameraZoom = 15.0f;

	[SerializeField] float _fallAnimationDelay = 0.5f;

	PlayerActor _actor;
	BuddyStats _prevHighlightedBuddy = null;

	[ReadOnly( "Respawn Position" )]
	[SerializeField] Transform _respawnTransform = null;

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

		_respawnTransform = FindObjectOfType<PlayerSpawnTag>().GetComponent<Transform>();

		DayCycleManager.RegisterEndOfDayCallback( Respawn );
		_textBox = FindObjectOfType<TextBox>();
	}

	void FixedUpdate()
	{
#if UNITY_EDITOR
		if ( Input.GetKeyDown( KeyCode.Y ) )
		{
			Respawn();
		}
#endif

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
			player.controls.SetFallAnimation( true );
			player.physics.lifter.gameObject.SetActive( false );
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
				player.animator.SetBool( "isInAir", true );

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

			player.animator.SetFloat( "moveSpeed", player.physics.normalizedGroundSpeed );
		}

		public override void Exit()
		{
			player.animator.ResetTrigger( "jump" );
			player.controls.SetFallAnimation( false );
			player.physics.lifter.gameObject.SetActive( true );
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
			player.animator.SetBool( "isInAir", true );
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

			player.animator.SetFloat( "moveSpeed", player.physics.normalizedGroundSpeed );
		}

		public override void Exit()
		{
			player.animator.SetBool( "isInAir", false );
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
			player.animator.SetBool( "isInAir", true );
			player.stats.StartUsingStat( Stat.Stamina );
			player.physics.StartClimbing();
			player.camera.zoomDistance = player.controls._climbingCameraZoom;
		}

		public override void Update()
		{
			if ( player.controls.holdButton &&
			     player.stats.CanUseStat( Stat.Stamina ) &&
			     player.physics.ClimbCheck() )
			{
				Vector3 moveInput = player.controls.GetMoveInput();
				player.physics.ClimbSurface( moveInput );
				player.animator.SetFloat( "moveSpeed", moveInput.magnitude );
				player.animator.SetFloat( "verticalSpeed", moveInput.z );
				player.animator.SetFloat( "horizontalSpeed", moveInput.x );
				player.animator.SetFloat( "slope", Mathf.Abs( Vector3.Dot( moveInput, Vector3.forward ) ) );
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
			player.camera.zoomDistance = player.controls._defaultCameraZoom;
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
					player.animator.SetTrigger( "jump" );
					player.physics.ChangeState( PhysicsStateType.Jumping );
					return;
				}
				else if ( player.controls.holdButton &&
				          player.stats.CanUseStat( Stat.Stamina ) &&
				          player.physics.ClimbCheck() )
				{
					player.physics.ChangeState( PhysicsStateType.Climbing );
					return;
				}
				else
				{
					RaycastHit hitInfo;
					if ( player.controls.RaycastForward( out hitInfo ) )
					{
						if( player.controls.useButton.down )
						{
							if ( player.cutting.CutCheck( hitInfo ) )
							{
								player.animator.SetTrigger( "cut" );
								return;
							}
							else if ( player.inventory.UseItemWithTarget( hitInfo ) )
							{
								// Feed buddy.
								return;
							}
							else if ( player.controls.InteractCheck( hitInfo ) )
							{
								// Interaction was done.
								return;
							}
						}
						else if( player.controls.holdButton.down )
						{
							// TODO: Convert this to an inventory item
							if ( player.inventory.CheckPutDownBuddy() )
							{
								// Buddy was put down
								return;
							}
							else if ( player.inventory.CheckPickUpBuddy( hitInfo ) )
							{
								// Buddy was picked up
								return;
							}
						}
					}

					if( player.inventory.CanUseItemWithoutTarget() )
					{
						if( player.controls.useButton.down )
						{
							player.inventory.UseItem();
						}

						player.controls.RemoveBuddyHighlight();
					}
					else if( !player.inventory.HasItem() )
					{
						player.controls.RemoveBuddyHighlight();
					}
					else
					{
						// Adds rim glow to buddy when looked at
						// TODO: This glow should only happen if holding an item that can be used on a buddy
						player.controls.HighlightBuddy( hitInfo );
					}
				}

				if ( player.controls.sprintButton )
				{
					player.camera.zoomDistance = player.controls._sprintingCameraZoom;
				}
				else
				{
					player.camera.zoomDistance = player.controls._defaultCameraZoom;
				}

				player.physics.GroundMovement( player.controls.GetMoveDirection(), player.controls.sprintButton );
				player.animator.SetFloat( "moveSpeed", player.physics.normalizedGroundSpeed );
			}
			else
			{
				player.physics.ChangeState( PhysicsStateType.Falling );
				player.physics.StartLateJumpTimer();
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
			player.animator.SetBool( "isInAir", true );
			player.animator.SetBool( "isGliding", true );
			player.parachuteControl.SetParachuteEnabled( true );
			player.camera.zoomDistance = player.controls._glidingCameraZoom;
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
			player.animator.SetBool( "isInAir", false );
			player.animator.SetBool( "isGliding", false );
			player.physics.StopGliding();
			player.parachuteControl.SetParachuteEnabled( false );
			player.camera.zoomDistance = player.controls._defaultCameraZoom;
		}
	}

	void SetupStateMethodMap()
	{
		_actor.physics.RegisterState( PhysicsStateType.Jumping,   new Jumping( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Falling,   new Falling( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Grounded,  new Grounded( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Climbing,  new Climbing( _actor ) );
		_actor.physics.RegisterState( PhysicsStateType.Gliding,   new Gliding( _actor ) );
	}
	#endregion

	public bool InteractCheck( RaycastHit hitInfo )
	{
		Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
		if ( interactable )
		{
			_textBox.SetText( interactable.interactText );
			return true;
		}

		return false;
	}

	public void HighlightBuddy( RaycastHit hitInfo )
	{
		if( hitInfo.transform )
		{
			BuddyStats highlightedBuddy = hitInfo.transform.GetComponentInChildren<BuddyStats>();
			if( highlightedBuddy )
			{
				if( _prevHighlightedBuddy && highlightedBuddy != _prevHighlightedBuddy )
				{
					_prevHighlightedBuddy.RemoveHighlight();
				}
				
				highlightedBuddy.SetHighlight();
				_prevHighlightedBuddy = highlightedBuddy;
			}
			else if( _prevHighlightedBuddy )
			{
				_prevHighlightedBuddy.RemoveHighlight();
			}
		}
		else if( _prevHighlightedBuddy )
		{
			_prevHighlightedBuddy.RemoveHighlight();
		}
	}

	public void RemoveBuddyHighlight()
	{
		if( _prevHighlightedBuddy )
		{
			_prevHighlightedBuddy.RemoveHighlight();
		}
	}

	public void SetFallAnimation( bool state )
	{
		if ( state )
		{
			Invoke( "_SetFallAnimation", _fallAnimationDelay );
		}
		else
		{
			CancelInvoke( "_SetFallAnimation" );
			_actor.animator.SetBool( "isInAir", false );
		}
	}

	void _SetFallAnimation()
	{
		_actor.animator.SetBool( "isInAir", true );
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

		if ( _actor.camera.cam )
		{
			inputVec = _actor.camera.cam.transform.TransformDirection( inputVec );
			inputVec.y = 0f;
		}

		return inputVec;
	}

	public void Respawn()
	{
		Teleport( _respawnTransform.position, _respawnTransform.rotation );
		SoundManager.Play2DSound( _respawnSound );
	}

	public void Teleport( Vector3 toPosition, Quaternion toRotation = new Quaternion(), bool snapCamera = true )
	{
		_actor.physics.Teleport( toPosition, toRotation );

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
