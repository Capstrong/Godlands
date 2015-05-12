using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PhysicsStateType
{
	Grounded = 0,
	Jumping,
	Falling,
	Climbing,
	Gliding,
	Dead,
}

public sealed class ActorPhysics : ActorComponent
{
	private Transform _transform;
	private Rigidbody _rigidbody;

	// States
	#region States
	[ReadOnly, SerializeField]
	PhysicsStateType _currentStateType = PhysicsStateType.Jumping;
	public PhysicsStateType currentStateType
	{
		get{ return _currentStateType; }
	}

	PhysicsState _currentState = new DefaultState();

	Dictionary<PhysicsStateType, PhysicsState> _stateMap = new Dictionary<PhysicsStateType, PhysicsState>();

	public delegate void StateCallback( PhysicsStateType physicsStateType );
	public StateCallback ExitStateCallback = delegate {};
	public StateCallback EnterStateCallback = delegate {};
	
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

	[SerializeField] float _groundedMoveSpeed = 6f;
	public float groundedMoveSpeed
	{
		get { return _groundedMoveSpeed; }
	}

	[SerializeField] float _sprintMoveSpeed = 12f;
	public float sprintMoveSpeed
	{
		get { return _sprintMoveSpeed; }
	}

	[SerializeField]
	float _jumpMoveSpeedChangeRate = 0.1f;

	[SerializeField] float _rollMoveSpeed = 6f;
	public float rollMoveSpeed
	{
		get { return _rollMoveSpeed; }
	}

	[SerializeField] float _climbMoveSpeed = 6f;

	public float normalizedMoveSpeed
	{
		get
		{
			Vector3 horizontalVelocity = _rigidbody.velocity.SetY( 0.0f );
			return horizontalVelocity.magnitude / sprintMoveSpeed;
		}
	}

	/**
	 * The last direction the actor moved in.
	 *
	 * Used for things like gliding or rolling where
	 * The actor needs to keep moving in the last direction.
	 */
	[ReadOnly("Move Vector")]
	[SerializeField] Vector3 _moveVec = Vector3.zero;
	#endregion

	#region Collisions
	[Space( 10 ), Header( "Collisions" )]
	public Collider bumper;
	public Collider lifter;
	public Collider climbBumper;
	private Transform _bumperTransform;
	private Rigidbody _bumperRigidbody;
	private Transform _lifterTransform;
	private Rigidbody _lifterRigidbody;
	#endregion

	#region Model Info
	[Space( 10 ), Header( "Model Info" )]
	[SerializeField] float _modelTurnSpeed = 7f;
	[Tooltip( "How much the actual velocity is factored into the look direction. 0 means look direction is entirely determined by the input, 1 means look direction is entirely determined by movement." )]
	float _lookIntentionWeight = 0.002f;
	bool _overrideLook = false;
	Vector3 _lookOverride = Vector3.zero;
	float _minLookVecTurnStrength = 0.0005f; // This stops jittering from  happening when moveVec is super small

	private Vector3 _lastPos;
	private Quaternion _desiredLook;
	#endregion

	#region Jumping
	[Space( 10 ), Header( "Jumping" )]
	[SerializeField] float jumpForce = 8.5f;

	[SerializeField] float _jumpCheckDistance = 1.0f;
	[Tooltip("Keep this under the radius of the bumper")]
	[SerializeField] float _jumpCheckRadius = 0.5f;

	[SerializeField] LayerMask _jumpLayer = 0;
	[SerializeField] float _minJumpDot = 0.4f;

	[SerializeField] float _lateJumpTime = 0.5f;
	bool _lateJumpDelay = false;

	[Tooltip( "The minimum delay between jump checks. Use this to prevent the player from immediately colliding with the ground after jumping." )]
	[SerializeField] float _jumpCheckDelayTime = 0.1f;
	bool _jumpCheckDelay = false;

	bool _isOnGround = false;
	#endregion

	#region Climbing
	[Space( 10 ), Header( "Climbing" )]
	[SerializeField] LayerMask _climbLayer = (LayerMask)0;
	[SerializeField] float _climbCheckRadius = 0.7f;
	[Range( 0.0f, 1.0f )]
	[SerializeField] float _leanTowardsSurface = 0.5f;

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

		_transform = GetComponent<Transform>();
		_rigidbody = GetComponent<Rigidbody>();

		_lastPos = _transform.position;

		_bumperTransform = bumper.GetComponent<Transform>();
		_bumperRigidbody = bumper.GetComponent<Rigidbody>();
		_lifterTransform = lifter.GetComponent<Transform>();
		_lifterRigidbody = lifter.GetComponent<Rigidbody>();

#if UNITY_EDITOR
		if ( bumper as CapsuleCollider )
		{
			DebugUtils.Assert( _jumpCheckRadius < ( (CapsuleCollider) bumper ).radius );
		}
		else if ( bumper as SphereCollider )
		{
			DebugUtils.Assert( _jumpCheckRadius < ( (SphereCollider) bumper ).radius );
		}
#endif

		InitializeStateMethodMap();
	}

	void Start()
	{
		ChangeState( PhysicsStateType.Grounded );
	}

	void FixedUpdate()
	{
		// Pre-Update stuff
		_desiredLook = _transform.rotation;

		// Update
		_currentState.Update();

		// Post-Update stuff
		ConstrainBumper();
		ConstrainLifter();
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

	/**
	 * Returns true if the actor is on the ground, false otherwise.
	 */
	public bool GroundedCheck()
	{
		_isOnGround = false;

		if ( !_jumpCheckDelay )
		{
			// spherecast down to detect when we're on the ground
			RaycastHit hit;
			Physics.SphereCast( _transform.position + Vector3.up,
			                    _jumpCheckRadius,
			                    -Vector3.up,
			                    out hit,
			                    _jumpCheckDistance,
			                    _jumpLayer );
			_isOnGround = ( hit.transform &&
			                Vector3.Dot( hit.normal, Vector3.up ) > _minJumpDot );
		}

		return _isOnGround;
	}

	public void GroundMovement( Vector3 moveVec, bool isSprinting = false )
	{
		FollowBumper();
		FollowLifter();

		if ( moveVec.IsZero() )
		{
			ComeToStop();
		}
		else
		{
			if ( isSprinting )
			{
				MoveAtSpeed( moveVec, sprintMoveSpeed );
			}
			else
			{
				MoveAtSpeed( moveVec, groundedMoveSpeed );
			}
		}

		CalculateDesiredLook();
	}

	/**
	 * Returns true if the actor is touching a climbable surface,
	 * false otherwise.
	 */
	public bool ClimbCheck()
	{
		Collider[] colliders = Physics.OverlapSphere( _transform.position, _climbCheckRadius, _climbLayer );
		if ( colliders.Length > 0 )
		{
			Collider nearestCol = colliders[0];
			float shortestDistance = ( nearestCol.transform.position - _transform.position ).sqrMagnitude;
			foreach ( Collider col in colliders )
			{
				float distance = ( col.transform.position - _transform.position ).sqrMagnitude;
				if ( distance < shortestDistance )
				{
					nearestCol = col;
					shortestDistance = distance;
				}
			}

			_climbTag = nearestCol.GetComponent<ClimbableTag>();
			_climbSurface = _climbTag.GetComponent<Transform>();
		}

		return colliders.Length > 0;
	}

	public void StartClimbing()
	{
		_rigidbody.useGravity = false;
		_rigidbody.velocity = Vector3.zero;
		lifter.gameObject.SetActive( false );
		bumper.gameObject.SetActive( false );
		climbBumper.gameObject.SetActive( true );
	}

	public void StopClimbing()
	{
		_climbSurface = null;
		_climbTag = null;
		_rigidbody.useGravity = true;
		lifter.gameObject.SetActive( true );
		bumper.gameObject.SetActive( true );
		climbBumper.gameObject.SetActive( false );
	}

	public void ClimbSurface( Vector3 movement )
	{
		DebugUtils.Assert( _climbSurface, "Cannot climb, surface is null" );
		DebugUtils.Assert( _climbTag, "Cannot climb, surface isn't tagged. This is probably a problem in ClimbCheck" );
		DebugUtils.Assert( _climbTag.xMovement || _climbTag.yMovement, "Climb tag does not allow horizontal or vertical movement. Tag needs to allow at least one of these." );

		Vector3 surfaceRelativeInput =
			_climbSurface.right * ( _climbTag.xMovement ? movement.x : 0.0f ) +
			_climbSurface.up    * ( _climbTag.yMovement ? movement.z : 0.0f );

		_moveVec = surfaceRelativeInput * _climbMoveSpeed;
		_rigidbody.velocity = _moveVec;

		// calculate desired look
		Vector3 lookVector = _climbSurface.forward;
		lookVector.y *= _leanTowardsSurface;
		_desiredLook = Quaternion.LookRotation( lookVector );
	}

	/**
	 * Returns true if the actor is able to jump, false otherwise.
	 */
	public bool JumpCheck()
	{
		return _isOnGround || _lateJumpDelay;
	}

	public void DoJump()
	{
		Vector3 curVelocity = _rigidbody.velocity.SetY( 0f );
		curVelocity.y = jumpForce;
		_rigidbody.velocity = curVelocity;
		_rigidbody.useGravity = true;

		StartJumpCheckDelayTimer();
	}

	/**
	 * Movement for both jumping and falling.
	 */
	public void AirMovement( Vector3 inputVec, bool forceUp = false )
	{
		_rigidbody.useGravity = true;

		FollowBumper();

		if ( inputVec.IsZero() && !forceUp )
		{
			ComeToStop();
		}
		else
		{
			_moveVec += inputVec * _jumpMoveSpeedChangeRate;
			_moveVec.y = 0.0f;

			if ( _moveVec.sqrMagnitude > _sprintMoveSpeed * _sprintMoveSpeed )
			{
				_moveVec = _moveVec.normalized * _sprintMoveSpeed;
			}

			if ( forceUp )
			{
				_moveVec.y = jumpForce;
			}
			else
			{
				_moveVec.y = _rigidbody.velocity.y;
			}

			_rigidbody.velocity = _moveVec;
		}

		CalculateDesiredLook();
	}

	public void StartGliding()
	{
		_rigidbody.useGravity = false;
	}

	public void StopGliding()
	{
		_rigidbody.useGravity = true;
	}

	public void GlideMovement( Vector3 inputVec )
	{
		FollowBumper();

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

		_rigidbody.velocity = _moveVec;

		CalculateDesiredLook();
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
		_bumperTransform.position = _transform.position;
		_bumperRigidbody.velocity = _rigidbody.velocity;
	}

	void ConstrainLifter()
	{
		_lifterTransform.position = _transform.position;
		_lifterRigidbody.velocity = _rigidbody.velocity;
	}

	void FollowBumper()
	{
		Vector3 constrainedPos = _bumperTransform.position;
		constrainedPos.y = _transform.position.y;
		_transform.position = constrainedPos;
	}

	void FollowLifter()
	{
		float constrainedY = _lifterTransform.position.y;
		_transform.position = _transform.position.SetY( constrainedY );

		float velocityY = _lifterRigidbody.velocity.y;
		_rigidbody.velocity = _rigidbody.velocity.SetY( ( velocityY + _rigidbody.velocity.y ) * 0.5f );
	}

	void MoveAtSpeed( Vector3 moveDir, float moveSpeed )
	{
		_moveVec = moveDir;

		_moveVec = moveDir * moveSpeed;
		_moveVec.y = _rigidbody.velocity.y;

		_rigidbody.velocity = _moveVec;
	}

	public void ComeToStop()
	{
		_moveVec = _rigidbody.velocity * _stoppingSpeed;

		_moveVec.y = _rigidbody.velocity.y;
		_rigidbody.velocity = _moveVec;
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
		_currentState.Exit();
		ExitStateCallback( _currentStateType );

		_currentStateType = toState;
		_currentState = _stateMap[_currentStateType];

		_currentState.Enter();
		EnterStateCallback( _currentStateType );
	}

	/**
	 * This calculates the desired look based on desired
	 * direction of movement and the actual direction
	 * of movement. If this is not how the look direction
	 * should be calculated for the current movement type,
	 * do not call this method.
	 */
	void CalculateDesiredLook()
	{
		Vector3 actualVelocity = _transform.position - _lastPos;
		actualVelocity.y = 0.0f;

		Vector3 intendedVelocity = _rigidbody.velocity;
		intendedVelocity.y = 0.0f;

		Vector3 weightedVelocity = Vector3.Lerp( actualVelocity, intendedVelocity, _lookIntentionWeight );

		Vector3 lookVec = ( _overrideLook ?
		                    _lookOverride :
		                    weightedVelocity );

		if ( lookVec.sqrMagnitude > _minLookVecTurnStrength * _minLookVecTurnStrength )
		{
			_desiredLook = Quaternion.LookRotation( lookVec, _transform.up );
		}
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
		_transform.rotation = Quaternion.Lerp(
		    _transform.rotation,
		    _desiredLook,
		    Time.deltaTime * _modelTurnSpeed );

		_lastPos = _transform.position;
	}

	void SetFallSpeed( float fallSpeed )
	{
		Vector3 moveVec = _rigidbody.velocity;
		moveVec.y = fallSpeed;
		_rigidbody.velocity = moveVec;
	}

	void EndLookOverride()
	{
		_overrideLook = false;
		_lookOverride = Vector3.zero;
	}

	public void Teleport( Vector3 toPosition, Quaternion toRotation = new Quaternion() )
	{
		_transform.position = toPosition;
		_transform.rotation = toRotation;
		_rigidbody.velocity = Vector3.zero;
	}

	#region Late Jump Timer
	public void StartLateJumpTimer()
	{
		_lateJumpDelay = true;
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
		_lateJumpDelay = false;
	}
	#endregion

	#region Jump Check Delay Timer
	void StartJumpCheckDelayTimer()
	{
		_jumpCheckDelay = true;
		Invoke( "EndJumpCheckDelay", _jumpCheckDelayTime );
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
		_jumpCheckDelay = false;
	}
	#endregion
}

public abstract class PhysicsState
{
	public abstract void Enter();
	public abstract void Update();
	public abstract void Exit();
}
