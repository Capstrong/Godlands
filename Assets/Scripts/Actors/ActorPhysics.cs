using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PhysicsStateType
{
	Grounded = 0,
	Jumping,
	Falling,
	Climbing,
	Gliding
}

public sealed class ActorPhysics : ActorComponent
{
	private new Transform transform;
	private new Rigidbody rigidbody;

	// States
	#region States
	[ReadOnly, SerializeField]
	PhysicsStateType _currentStateType = PhysicsStateType.Jumping;
	PhysicsState _currentState = new DefaultState();
	Dictionary<PhysicsStateType, PhysicsState> _stateMap = new Dictionary<PhysicsStateType, PhysicsState>();
	
	public class DefaultState : PhysicsState
	{
		public override void Enter()  { }
		public override void Update() { }
		public override void Exit()   { }
	}
	#endregion

	#region Movement
	[Header( "Movement" )]
	[SerializeField] float _stoppingSpeed = 0.001f;

	[SerializeField] float _stopMoveTime = 0.3f;
	float _stopMoveTimer = 0f;

	[SerializeField] float _groundedMoveSpeed = 6f;
	public float groundedMoveSpeed
	{
		get { return _groundedMoveSpeed; }
	}

	[SerializeField] float _jumpMoveSpeed = 6f;

	[SerializeField] float _rollMoveSpeed = 6f;
	public float rollMoveSpeed
	{
		get { return _rollMoveSpeed; }
	}

	[SerializeField] float _climbMoveSpeed = 6f;

	/**
	 * The last direction the actor moved in.
	 *
	 * Used for things like gliding or rolling where
	 * The actor needs to keep moving in the last direction.
	 */
	Vector3 _moveVec = Vector3.zero;
	#endregion

	#region Collisions
	[Space( 10 ), Header( "Collisions" )]
	public Collider bumper;
	public Collider lifter;
	public Collider climbBumper;
	private Transform _bumperTransform;
	private Rigidbody _bumperRigidbody;
	#endregion

	#region Model Info
	[Space( 10 ), Header( "Model Info" )]
	[SerializeField] float _modelTurnSpeed = 7f;
	[Tooltip( "How much the actual velocity is factored into the look direction. 0 means look direction is entirely determined by the input, 1 means look direction is entirely determined by movement." )]
	float _lookIntentionWeight = 0.002f;
	bool _overrideLook = false;
	Vector3 _lookOverride = Vector3.zero;

	private Vector3 _lastPos;
	#endregion

	#region Jumping
	[Space( 10 ), Header( "Jumping" )]
	[SerializeField] float jumpForce = 8.5f;

	[SerializeField] float _jumpCheckDistance = 1.0f;
	[SerializeField] float _jumpCheckRadius = 0.5f;

	[SerializeField] LayerMask _jumpLayer = 0;
	[SerializeField] float _minJumpDot = 0.4f;

	[SerializeField] float _jumpColCheckTime = 0.5f;
	float _jumpColCheckTimer = 0.0f; // TODO convert this to an invoke

	[SerializeField] float _lateJumpTime = 0.5f;
	bool _lateJump = false;

	[Tooltip( "The minimum delay between jump checks. Use this to prevent the player from immediately colliding with the ground after jumping." )]
	[SerializeField] float _jumpCheckDelay = 0.1f;
	bool _isJumpCheckDelay = false;

	bool _isOnGround = false;
	#endregion

	#region Climbing
	[Space( 10 ), Header( "Climbing" )]
	[SerializeField] LayerMask _climbLayer = (LayerMask)0;
	[SerializeField] float _climbCheckRadius = 0.7f;
	[Range( 0.0f, 1.0f )]
	[SerializeField] float _leanTowardsSurface = 0.5f;

	//[SerializeField] float _climbCheckTime = 0.2f;
	float _climbCheckTimer = 1f;

	Transform _climbSurface = null;
	ClimbableTag _climbTag;
	#endregion

	#region Gliding
	[Space( 10 ), Header( "Gliding" )]
	[SerializeField] float _glideHorizontalSpeed = 5.0f;
	[SerializeField] float _glideDescentRate = 1.0f;
	[SerializeField] float _glideTurnRate = 0.5f;
	#endregion

	public override void Awake()
	{
		base.Awake();

		transform = GetComponent<Transform>();
		rigidbody = GetComponent<Rigidbody>();

		_lastPos = transform.position;

		_bumperTransform = bumper.GetComponent<Transform>();
		_bumperRigidbody = bumper.GetComponent<Rigidbody>();

		InitializeStateMethodMap();
	}

	void Start()
	{
		ChangeState( PhysicsStateType.Grounded );
	}

	void FixedUpdate()
	{
		// Pre-Update stuff
		FollowBumper();

		// Update
		_currentState.Update();

		// Post-Update stuff
		ConstrainBumper();
		OrientSelf();
	}

	public void RegisterState( PhysicsStateType state, PhysicsState method )
	{
		if ( _stateMap.ContainsKey( state ) )
		{
			_stateMap[state] = method;
		}
		else
		{
			_stateMap.Add( state, method );
		}
	}

	public bool GroundedCheck()
	{
		_isOnGround = false;

		if ( !_isJumpCheckDelay )
		{
			// spherecast down to detect when we're on the ground
			RaycastHit hit;
			Physics.SphereCast( transform.position + Vector3.up,
			                    _jumpCheckRadius,
			                    -Vector3.up * _jumpCheckDistance,
			                    out hit,
			                    _jumpCheckDistance,
			                    _jumpLayer );
			_isOnGround = ( hit.transform &&
			                Vector3.Dot( hit.normal, Vector3.up ) > _minJumpDot );

			if ( _isOnGround &&
			     !IsInState( PhysicsStateType.Climbing ) )
			{
				if ( !IsInState( PhysicsStateType.Grounded ) )
				{
					_stopMoveTimer = 0f;
				}

				CancelLateJumpTimer();
			}
			else if ( IsInState( PhysicsStateType.Grounded ) )
			{
				StartLateJumpTimer();
			}
		}

		return _isOnGround;
	}

	public void GroundMovement( Vector3 moveVec )
	{
		MoveAtSpeed( moveVec, groundedMoveSpeed );
	}

	public void ComeToStop()
	{
		_moveVec = rigidbody.velocity * _stoppingSpeed;

		_moveVec.y = rigidbody.velocity.y;
		rigidbody.velocity = _moveVec;

		if ( _jumpColCheckTimer > _jumpColCheckTime )
		{
			if ( IsInState( PhysicsStateType.Grounded ) )
			{
				if ( _stopMoveTimer >= _stopMoveTime )
				{
					rigidbody.useGravity = false;
					SetFallSpeed( 0.0f );
				}

				_stopMoveTimer += Time.deltaTime;
			}
			else
			{
				rigidbody.useGravity = true;
			}
		}
	}

	public bool ClimbCheck()
	{
		_climbCheckTimer += Time.deltaTime;

		bool climbing = false;
		//if ( climbCheckTimer > climbCheckTime )
		{
			Collider[] colliders = Physics.OverlapSphere( transform.position, _climbCheckRadius, _climbLayer );
			if ( colliders.Length > 0 )
			{
				Collider nearestCol = colliders[0];
				float shortestDistance = float.MaxValue;
				foreach ( Collider col in colliders )
				{
					float distance =  ( col.transform.position - transform.position ).sqrMagnitude;
					if ( distance < shortestDistance )
					{
						nearestCol = col;
						shortestDistance = distance;
					}
				}

				_climbTag = nearestCol.GetComponent<ClimbableTag>();
				_climbSurface = _climbTag.GetComponent<Transform>();

				climbing = true;
			}
			else
			{
				StopClimbing();
				climbing = false;
			}

			_climbCheckTimer = 0f;
		}
		//else
		//{
		//	climbing = true;
		//}
		
		return climbing;
	}

	public void StartClimbing()
	{
		rigidbody.useGravity = false;
		rigidbody.velocity = Vector3.zero;
		lifter.gameObject.SetActive( false );
		bumper.gameObject.SetActive( false );
		climbBumper.gameObject.SetActive( true );
	}

	public void StopClimbing()
	{
		_climbSurface = null;
		_climbTag = null;
		rigidbody.useGravity = true;
		lifter.gameObject.SetActive( true );
		bumper.gameObject.SetActive( true );
		climbBumper.gameObject.SetActive( false );
	}

	public void ClimbSurface( Vector3 movement )
	{
		DebugUtils.Assert( _climbSurface, "Cannot climb, surface is null" );

		if ( _climbTag )
		{
			DebugUtils.Assert( _climbTag.xMovement || _climbTag.yMovement );

			Vector3 surfaceRelativeInput =
				_climbSurface.right * ( _climbTag.xMovement ? movement.x : 0.0f ) +
				_climbSurface.up * ( _climbTag.yMovement ? movement.z : 0.0f );

			Debug.DrawRay( transform.position, surfaceRelativeInput * 10.0f );

			_moveVec = surfaceRelativeInput * _climbMoveSpeed;

			rigidbody.velocity = _moveVec;
		}
		else
		{
			Debug.LogError( "Cannot climb, surface isn't tagged. This is probably a problem in ClimbCheck" );
		}
	}

	public bool JumpCheck()
	{
		if ( _isOnGround || _lateJump )
		{
			_lateJump = false;

			Vector3 curVelocity = rigidbody.velocity;
			curVelocity.y = jumpForce;
			rigidbody.velocity = curVelocity;
			rigidbody.useGravity = true;

			_jumpColCheckTimer = 0.0f;
			StartJumpCheckDelayTimer();

			return true;
		}

		return false;
	}

	public void JumpMovement( Vector3 inputVec )
	{
		if ( inputVec.IsZero() )
		{
			ComeToStop();
		}
		else
		{
			_moveVec = inputVec * _jumpMoveSpeed;
			_moveVec.y = rigidbody.velocity.y;

			rigidbody.velocity = _moveVec;
		}
	}

	public void StartGliding()
	{
		rigidbody.useGravity = false;
	}

	public void StopGliding()
	{
		rigidbody.useGravity = true;
	}

	public void GlideMovement( Vector3 inputVec )
	{
		if ( !inputVec.IsZero() )
		{
			_moveVec = Vector3.RotateTowards( _moveVec,
			                                  inputVec,
			                                  _glideTurnRate * Time.deltaTime,
			                                  0.0f );
		}

		_moveVec.y = 0.0f;
		_moveVec = _moveVec.normalized * _glideHorizontalSpeed;
		_moveVec.y = -_glideDescentRate;

		rigidbody.velocity = _moveVec;
	}

	public void OverrideLook( Vector3 lookDir, float time )
	{
		_lookOverride = lookDir;
		_lookOverride.y = 0.0f;
		_overrideLook = true;
		CancelInvoke( "EndLookOverride" );
		Invoke( "EndLookOverride", time );
	}

	void ConstrainBumper()
	{
		_bumperTransform.position = transform.position;
		_bumperRigidbody.velocity = rigidbody.velocity;
	}

	void FollowBumper()
	{
		if ( !IsInState( PhysicsStateType.Climbing ) )
		{
			Vector3 constrainedPos = _bumperTransform.position;
			constrainedPos.y = transform.position.y;
			transform.position = constrainedPos;
		}
	}

	void MoveAtSpeed( Vector3 moveDir, float moveSpeed )
	{
		_moveVec = moveDir;

		_moveVec = moveDir * moveSpeed;
		_moveVec.y = rigidbody.velocity.y;

		rigidbody.velocity = _moveVec;
	}

	/**
	 * Provide reasonable default values for the state methods.
	 * 
	 * If the state method map has already been configured
	 * before this is called, the methods will not be overriden.
	 */
	void InitializeStateMethodMap()
	{
		foreach ( PhysicsStateType state in System.Enum.GetValues( typeof( PhysicsStateType ) ) )
		{
			if ( !_stateMap.ContainsKey( state ) )
			{
				_stateMap.Add( state, new DefaultState() );
			}
		}
	}

	/**
	 * Transition between physics states.
	 * 
	 * Note: This immediately calls the Exit() method of the
	 * old state and then the Enter() method of the new state.
	 * If you call ChangeState() multiple times in one frame
	 * the state transition will take place multiple times,
	 * including whatever side effects that may entail.
	 * Best practices is to ensure that you only transition
	 * states at most once per frame.
	 */
	public void ChangeState( PhysicsStateType toState )
	{
		_currentStateType = toState;

		_currentState.Exit();
		_currentState = _stateMap[_currentStateType];
		_currentState.Enter();
	}

	/**
	 * Orients the model to face the direction of movement
	 *
	 * Orientation is reactive to velocity, meaning the
	 * model will face the direction of movement, rather
	 * than the input direction.
	 * 
	 * When climbing a wall, the model will also slightly
	 * lean to match the orientation of the wall.
	 */
	void OrientSelf()
	{
		Quaternion desiredLook = transform.rotation;
		if ( IsInState( PhysicsStateType.Climbing ) )
		{
			Vector3 lookVector = _climbSurface.forward;
			lookVector.y *= _leanTowardsSurface;
			desiredLook = Quaternion.LookRotation( lookVector );
		}
		else
		{
			Vector3 actualVelocity = transform.position - _lastPos;
			actualVelocity.y = 0.0f;

			Vector3 intendedVelocity = rigidbody.velocity;
			intendedVelocity.y = 0.0f;

			Vector3 lookVec =
				( _overrideLook ?
				_lookOverride :
				Vector3.Lerp( actualVelocity, intendedVelocity, _lookIntentionWeight ) );

			if ( !lookVec.IsZero() )
			{
				desiredLook = Quaternion.LookRotation( lookVec, transform.up );
			}
		}

		transform.rotation = Quaternion.Lerp(
		    transform.rotation,
		    desiredLook,
		    Time.deltaTime * _modelTurnSpeed );

		_lastPos = transform.position;
	}

	bool IsInState( PhysicsStateType checkState )
	{
		return _currentStateType == checkState;
	}

	void SetFallSpeed( float fallSpeed )
	{
		Vector3 moveVec = rigidbody.velocity;
		moveVec.y = fallSpeed;
		rigidbody.velocity = moveVec;
	}

	void EndLookOverride()
	{
		_overrideLook = false;
		_lookOverride = Vector3.zero;
	}

	#region Late Jump Timer
	void StartLateJumpTimer()
	{
		_lateJump = true;
		Invoke( "EndLateJump", _lateJumpTime );
	}

	void CancelLateJumpTimer()
	{
		if ( IsInvoking( "EndLateJump" ) )
		{
			CancelInvoke( "EndLateJump" );
		}
		EndLateJump();
	}

	void EndLateJump()
	{
		_lateJump = false;
	}
	#endregion

	#region Jump Check Delay Timer
	void StartJumpCheckDelayTimer()
	{
		_isJumpCheckDelay = true;
		Invoke( "EndJumpCheckDelay", _jumpCheckDelay );
	}

	void CancelJumpCheckDelayTimer()
	{
		if ( IsInvoking( "EndJumpCheckDelay" ) )
		{
			CancelInvoke( "EndJumpCheckDelay" );
		}
		EndJumpCheckDelay();
	}

	void EndJumpCheckDelay()
	{
		_isJumpCheckDelay = false;
	}
	#endregion
}

public abstract class PhysicsState
{
	public abstract void Enter();
	public abstract void Update();
	public abstract void Exit();
}
