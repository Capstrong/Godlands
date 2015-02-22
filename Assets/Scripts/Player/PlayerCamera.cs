using UnityEngine;
using System.Collections;

public class PlayerCamera : ActorComponent 
{
	public Camera cam = null;

	// Rotation
	[SerializeField] MinMaxF _rotationBounds = new MinMaxF( -30f, 50f );
	float _rotationBoundsWidth = 50f; // This determines how close to the bounds rotation will be dampened

	[SerializeField] float _vertRotationSpeed = 2f; // Vertical
	[SerializeField] float _horRotationSpeed = 2f; // Horizontal

	// Focus
	Vector3 _headOffset = new Vector3( 0f, 1.75f, 0f );
	Vector3 _focusPoint = Vector3.zero; // This is what the camera looks at

	[SerializeField] float _focusShiftSpeed = 7f; // How quickly does the focus move onto a new spot
	[SerializeField] float _focusFollowSpeed = 12f; // How quickly does the camera follow its focus
	[SerializeField] Vector3 _focusOffset = new Vector3( 0f, 0f, 0.4f ); // How far offset is the camera it's intended position

	// Zoom
	float _currentZoomDistance = 0f;
	[SerializeField] float _defaultZoomDistance = 10f; // Probably should add zoom distance for closeup mode, far away mode, etc
	[SerializeField] float _targetZoomDistance = 10f;

	[SerializeField] MinMaxF _zoomDistanceBounds = new MinMaxF( 1f, 25f );

	// Turn Assist
	[SerializeField] AnimationCurve _turnAssistCurve = new AnimationCurve();
	[SerializeField] float _turnAssistMinAngle = 20f; // This is the minimum angle where your turns are assisted
	[SerializeField] float _turnAssistTurnSpeed = 1f;

	// Collision Zoom
	[SerializeField] LayerMask _collisionLayer = 0;

	public override void Awake()
	{
		base.Awake();

		if( !cam )
		{
			if( Camera.main )
			{
				cam = Camera.main;
			}
			else
			{
				cam = FindObjectOfType<Camera>();
			}
		}
	}

	void FixedUpdate ()
	{
		_focusPoint = transform.position + _headOffset;

		_zoomDistanceBounds.Clamp( ref _targetZoomDistance );
		_currentZoomDistance = Mathf.Lerp( _currentZoomDistance,
		                                   _targetZoomDistance,
		                                   _focusShiftSpeed * Time.deltaTime );

		CameraControl();
	}

	void Update()
	{
		//Lock cursor
		if(Input.GetMouseButtonDown(0))
		{
			Screen.lockCursor = true;
		}

		if ( Input.GetKeyDown( KeyCode.T ) )
		{
			Application.CaptureScreenshot( "Screenshot_" + System.DateTime.Now.ToString( "yyyy.MM.dd.HH.mm.ss" ) + ".png", 4 );
		}
	}

	void CameraControl()
	{
		// X - Camera movement is smooth

		// X - Camera rotates on X and Y around focus point
			// Y axis
				// Camera can rotate freely around Y axis
				// Rotation around axis is mouseY * yMod
				// Rotation gets harder as you get close to the limit
			// X axis
				// Camera has limits on it's rotation
				// Limits are up and down values
				// Rotation around axis is mouseX * xMod

		// X - Camera tries to maintain line of sight w/ the focus point
			// if not player controlled, Camera will rotate around obstacles to keep view of its focus
				// Cast rays to find the easiest path
				// Worst case: cast a ray from the player towards our focus and zoom to the closest hit

		// X - Camera tries to maintain offset from focus point

		// Camera zooms to try and keep interesting elements on screen
			// For interesting elements near the player, zoom out to show more

		// Different modes of viewing:
			// During platforming, we want a far out view
			// While running, camera zooms in
			// When dealing with buddies, we want a close up side-view
			// Toggle between zoom/not zoom mode w/ R3
				// Zoomed in mode has camera offset a bit to the right

		float xMouseInput = InputUtils.GetAxis( "Mouse X" );
		float yMouseInput = InputUtils.GetAxis( "Mouse Y" );

		Vector3 actorHead = transform.position + _headOffset;
		Vector3 actorToCam = cam.transform.position - actorHead;

		if( WadeUtils.IsZero( Mathf.Abs( xMouseInput ) ) &&
		    WadeUtils.IsZero( Mathf.Abs( yMouseInput ) ) )
		{
			MoveCameraBehindPlayer( actorToCam );
		}
		else
		{
			cam.transform.RotateAround( transform.position,
			                            cam.transform.up,
			                            xMouseInput * _horRotationSpeed );
			
			cam.transform.RotateAround( transform.position,
			                            cam.transform.right,
			                            yMouseInput * _vertRotationSpeed * GetRotationSpeedModifier() );
		}

		KeepLineOfSight( actorHead, actorToCam );

		// Lock Z rotation
		Vector3 camEuler = cam.transform.eulerAngles;
		camEuler.z = 0f;
		cam.transform.eulerAngles = camEuler;

		actor.SetRenderers( !IsCameraInsidePlayer() );
	
		Vector3 currentOffset = _focusOffset * _currentZoomDistance;
		currentOffset += currentOffset * cam.nearClipPlane;

		cam.transform.position = Vector3.Lerp( cam.transform.position, 
		                                       (_focusPoint - cam.transform.rotation * currentOffset) + Vector3.up * cam.nearClipPlane,
		                                       _focusFollowSpeed * Time.deltaTime );
	}

	bool IsCameraInsidePlayer()
	{
		return _currentZoomDistance < _zoomDistanceBounds.min;
	}

	float GetRotationSpeedModifier()
	{
		float yInput = InputUtils.GetAxis( "Mouse Y" );
		float xEuler = cam.transform.eulerAngles.x;
		float speedMod = 1f;

		// Bottom rotation
		if( xEuler > 270f && yInput < 0f ) 
		{
			xEuler -= 360f + _rotationBounds.min;
			speedMod = Mathf.Lerp( 0f, 1f, Mathf.InverseLerp( 0f, _rotationBoundsWidth, xEuler ) );
		}

		// Top rotation
		if( xEuler < 90f && yInput > 0f )
		{
			xEuler -= _rotationBounds.min;
			speedMod = Mathf.Lerp( 1f, 0f, Mathf.InverseLerp( _rotationBounds.Difference - _rotationBoundsWidth,
			                                              	  _rotationBounds.Difference, 
			                                              	  xEuler ) );
		}

		return speedMod;
	}

	void MoveCameraBehindPlayer( Vector3 actorToCam )
	{
		float xMoveInput = InputUtils.GetAxis( "Horizontal" );

		float angleDistance = Mathf.Acos( Vector3.Dot( -actorToCam.normalized, rigidbody.velocity.normalized ) ) * Mathf.Rad2Deg;
	
		float turnSpeedMod = Mathf.InverseLerp( _turnAssistMinAngle, 180f, angleDistance );
		turnSpeedMod = _turnAssistCurve.Evaluate( turnSpeedMod );

		cam.transform.RotateAround( transform.position,
		                            cam.transform.up,
		                            turnSpeedMod * _turnAssistTurnSpeed * xMoveInput );

		_targetZoomDistance = _defaultZoomDistance;
	}
	
	void KeepLineOfSight( Vector3 actorHead, Vector3 actorToCam )
	{
		float nearestDist = Mathf.Infinity;
		RaycastHit[] hits = Physics.RaycastAll( actorHead,
		                                        actorToCam,
		                                        _zoomDistanceBounds.max,
		                                        _collisionLayer );

		if(hits.Length > 0)
		{
			foreach( RaycastHit hit in hits )
			{
				float hitDist = (transform.position - hit.point).sqrMagnitude;
				if(hitDist < nearestDist && hit.transform && !hit.transform.GetComponent<NoZoom>())
				{
					nearestDist = hitDist;
				}
			}

			_targetZoomDistance = Mathf.Sqrt( nearestDist );
		}
		else
		{
			_targetZoomDistance = _defaultZoomDistance;
		}
	}
}
