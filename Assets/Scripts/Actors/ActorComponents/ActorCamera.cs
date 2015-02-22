using UnityEngine;
using System.Collections;

public class ActorCamera : ActorComponent 
{
	public Camera cam;

	public override void Awake()
	{
		base.Awake();
	}

	void FixedUpdate ()
	{
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
		// Camera movement is smooth

		// Camera rotates on X and Y around focus point
			// Y axis
				// Camera can rotate freely around Y axis
				// Rotation around axis is mouseY * yMod
				// Rotation gets harder as you get close to the limit
			// X axis
				// Camera has limits on it's rotation
				// Limits are up and down values
				// Rotation around axis is mouseX * xMod

		// Camera tries to maintain line of sight w/ the focus point
			// if not player controlled, Camera will rotate around obstacles to keep view of its focus
				// Cast rays to find the easiest path
				// Worst case: cast a ray from the player towards our focus and zoom to the closest hit

		// Camera tries to maintain offset from focus point

		// Camera zooms to try and keep interesting elements on screen
			// For interesting elements near the player, zoom out to show more

		// Different modes of viewing:
			// During platforming, we want a far out view
			// While running, camera zooms in
			// When dealing with buddies, we want a close up side-view
			// Toggle between zoom/not zoom mode w/ R3
				// Zoomed in mode has camera offset a bit to the right

	}

//	void CameraControl()
//	{
//		if ( cam )
//		{
//			cam.transform.RotateAround( transform.position,
//			                            cam.transform.right,
//			                            InputUtils.GetAxis( "Mouse Y" ) * rotSpeed.y );
//			cam.transform.RotateAround( transform.position,
//			                            cam.transform.up,
//			                            InputUtils.GetAxis( "Mouse X" ) * rotSpeed.x );
//
//			Vector3 camEuler = cam.transform.eulerAngles;
//			camEuler.z = 0f;
//
//			if ( camEuler.x > rotBounds.min && camEuler.x < 300 )
//			{
//				camEuler.x = rotBounds.min;
//			}
//			if ( camEuler.x < rotBounds.max && camEuler.x > 100 )
//			{
//				camEuler.x = rotBounds.max;
//			}
//
//			GetMinCameraDistance();
//
//			if ( currentCamDist < minCamDist )
//			{
//				actor.SetRenderers( false );
//			}
//			else
//			{
//				actor.SetRenderers( true );
//			}
//
//			Vector3 currentOffset = camOffset * currentCamDist;
//			targetPos = Vector3.Lerp( transform.position - cam.transform.rotation * currentOffset,
//			                          transform.position + Quaternion.Euler( -150.0f, camEuler.y, camEuler.z ) * currentOffset,
//			                          camEuler.x < 80.0f ? camEuler.x / 75.0f : 0.0f );
//
//			//RaycastHit hit = WadeUtils.RaycastAndGetInfo(cam.transform.position,
//			//                                             -cam.transform.up,
//			//                                             groundLayer,
//			//                                             groundCheckDist);
//			//if(hit.transform)
//			//{
//			//	targetPos.y = hit.point.y + groundCheckDist;
//			//}
//
//			cam.transform.position = targetPos + hitPosOffset;
//			cam.transform.eulerAngles = camEuler;
//		}
//	}
//
//	void GetMinCameraDistance()
//	{
//		// Need to get an array of ALL hit things and then use the closest valid one as our hit
//		// Currently the check stops as soon as anything is hit, even if invalid
//
//		Vector3 headPos = transform.position + transform.up * 1.5f;
//		Vector3 viewVec = ((transform.position - cam.transform.rotation * camOffset * defaultCamDist) - headPos).normalized * defaultCamDist;
//
//		float nearestDist = Mathf.Infinity;
//		RaycastHit closestHit = new RaycastHit();
//		RaycastHit[] hits = Physics.RaycastAll(headPos, viewVec, viewVec.magnitude, camLayer);
//		foreach(RaycastHit hit in hits)
//		{
//			float hitDist = Vector3.Distance(headPos, hit.point);
//			if(hitDist < nearestDist && hit.transform && !hit.transform.GetComponent<NoZoom>())
//			{
//				nearestDist = hitDist;
//				closestHit = hit;
//			}
//		}
//
//		WadeUtils.Lerp(ref currentCamDist, Mathf.Min(nearestDist, defaultCamDist), Time.deltaTime * zoomSpeed);
//	}
}
