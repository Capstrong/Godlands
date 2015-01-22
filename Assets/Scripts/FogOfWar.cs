using UnityEngine;
using UnityEditor;
using System.Collections;

public class FogOfWar : MonoBehaviour
{
	public GameObject cloudPrefab;

	public float radius;
	public float height;

	public int numClouds;

	private new Transform transform;

	void Awake()
	{
		transform = GetComponent<Transform>();
	}

	void Start()
	{
		// generate a ring of the cloud prefabs
		for ( int count = 0; count < numClouds; ++count )
		{
			Vector3 cloudOffset = Random.insideUnitCircle.normalized;
			cloudOffset.z = cloudOffset.y;
			cloudOffset.y = 0;
			Transform cloud = ( GameObject.Instantiate(
				cloudPrefab,
				cloudOffset * radius,
				Quaternion.identity ) as GameObject ).GetComponent<Transform>();
			cloud.SetParent( transform, false );
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(GetComponent<Transform>().position + Vector3.up * .5f, "S.png", true);
	}
}
