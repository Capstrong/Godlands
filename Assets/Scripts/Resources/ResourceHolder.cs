using UnityEngine;
using System.Collections;

public class ResourceHolder : MonoBehaviour
{
	public GameObject resource;

	float resourceHeightOffset = 0.17f;

	void Awake()
	{
		resource = WadeUtils.Instantiate( resource, Vector3.up * resourceHeightOffset, Quaternion.identity );
		resource.GetComponent<Transform>().SetParent( transform, false );
		resource.GetComponent<InventoryItem>().beaconObj = gameObject;
	}
}
