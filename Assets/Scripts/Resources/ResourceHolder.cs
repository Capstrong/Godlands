using UnityEngine;
using System.Collections;

public class ResourceHolder : MonoBehaviour
{
	public GameObject resource;

	void Start()
	{
		resource = WadeUtils.Instantiate( resource, Vector3.zero, Quaternion.identity );
		resource.GetComponent<Transform>().SetParent( transform, false );
	}
}
