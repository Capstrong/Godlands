using UnityEngine;
using System.Collections;

public class ActorCamera : ActorComponent 
{
	public Camera cam;

	[SerializeField] float followSpeed = 15f;
	[SerializeField] float zoomSpeed = 100f;

	[SerializeField] float defaultCamDist = 10.6f;
	[SerializeField] float minCamDist = 5f;
	float currentCamDist;

	[SerializeField] LayerMask camLayer;
	[SerializeField] LayerMask groundLayer;
	[SerializeField] LayerMask waterLayer;

	bool underWater = false;

	Vector3 camOffsetRatio = new Vector3(0.0f, -2.5f, 10.0f);
	Vector3 camOffset = Vector3.zero;

	Vector3 targetPos = Vector3.zero;

	Vector3 hitPosOffset = Vector3.zero;
	[SerializeField] float hitPosOffsetDist = 0.5f;

	[SerializeField] Vector2 rotSpeed = Vector2.one;
	[SerializeField] MinMax rotBounds = new MinMax(40f, 350f);

	Renderer modelRend;

	void Awake()
	{
		currentCamDist = defaultCamDist;
		camOffset = camOffsetRatio.normalized;
	}

	void FixedUpdate ()
	{
		CameraControl();
	}

	void Update()
	{
//		if(!Screen.lockCursor && Input.mousePresent)
//		{
//			Screen.lockCursor = true;
//		}

		//Lock cursor
		if(Input.GetMouseButtonDown(0))
		{
			Screen.lockCursor = true;
		}
	}

	void ToggleUnderWaterMode(bool setOn)
	{
		RenderSettings.fog = setOn;
	}

	void UnderWaterTest()
	{
		RaycastHit hit = WadeUtils.RaycastAndGetInfo(cam.transform.position, Vector3.up, waterLayer, 10f);
		if(hit.transform)
		{
			if(!underWater)
			{
				underWater = true;
				ToggleUnderWaterMode(true);
			}
		}
		else if(underWater)
		{
			underWater = false;
			ToggleUnderWaterMode(false);
		}
	}

	void CameraControl()
	{
		if(cam)
		{
			UnderWaterTest();

			cam.transform.RotateAround(transform.position, 
			                           cam.transform.right, 
			                           Input.GetAxis("Mouse Y" + WadeUtils.platformName) * rotSpeed.y);
			cam.transform.RotateAround(transform.position, 
			                           cam.transform.up, 
			                           Input.GetAxis("Mouse X" + WadeUtils.platformName) * rotSpeed.x);

			Vector3 camEuler = cam.transform.eulerAngles;
			camEuler.z = 0f;

			if(camEuler.x > rotBounds.min && camEuler.x < 300) 
			{
				camEuler.x = rotBounds.min;
			}
			if(camEuler.x < rotBounds.max && camEuler.x > 100) 
			{
				camEuler.x = rotBounds.max;
			}

			GetMinCameraDistance();

			if(actor.AreRenderersOn() && currentCamDist < minCamDist)
			{
				actor.ToggleRenderers(false);
			}
			else if (!actor.AreRenderersOn() && currentCamDist >= minCamDist)
			{
				actor.ToggleRenderers(true);
			}

			Vector3 currentOffset = camOffset * currentCamDist;
			targetPos = Vector3.Lerp(transform.position - cam.transform.rotation * currentOffset, 
			                         transform.position + Quaternion.Euler(-150.0f, camEuler.y, camEuler.z) * currentOffset,
			                         camEuler.x < 80.0f ? camEuler.x/75.0f : 0.0f);

//			RaycastHit hit = WadeUtils.RaycastAndGetInfo(cam.transform.position, 
//			                                             -cam.transform.up, 
//			                                             groundLayer, 
//			                                             groundCheckDist);
//			if(hit.transform)
//			{
//				targetPos.y = hit.point.y + groundCheckDist;
//			}

			cam.transform.position = targetPos + hitPosOffset;
			cam.transform.eulerAngles = camEuler;
		}
	}

	void GetMinCameraDistance()
	{
		// Need to get an array of ALL hit things and then use the closest valid one as our hit
		// Currently the check stops as soon as anything is hit, even if invalid

		Vector3 headPos = transform.position + transform.up * 1.5f;
		Vector3 viewVec = ((transform.position - cam.transform.rotation * camOffset * defaultCamDist) - headPos).normalized * defaultCamDist;

		float nearestDist = Mathf.Infinity;
		RaycastHit closestHit = new RaycastHit();
		RaycastHit[] hits = Physics.RaycastAll(headPos, viewVec, viewVec.magnitude, camLayer);
		foreach(RaycastHit hit in hits)
		{
			float hitDist = Vector3.Distance(headPos, hit.point);
			if(hitDist < nearestDist && hit.transform && !hit.transform.GetComponent<NoZoom>())
			{
				nearestDist = hitDist;
				closestHit = hit;
			}
		}

		if(closestHit.transform)
		{
			hitPosOffset = closestHit.normal * hitPosOffsetDist;
		}
		else
		{
			hitPosOffset = Vector3.zero;
		}

		WadeUtils.Lerp(ref currentCamDist, Mathf.Min(nearestDist, defaultCamDist), Time.deltaTime * zoomSpeed);
	}
}
