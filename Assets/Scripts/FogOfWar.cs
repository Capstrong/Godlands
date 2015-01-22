using UnityEngine;
using UnityEditor;
using System.Collections;

public class FogOfWar : MonoBehaviour
{
	public GameObject cloudPrefab;

	public float radius;
	public float radiusError;
	public float height;

	[Range( 0.0f, 360.0f )]
	public float cloudIncrement;

	public float minScale;
	public float maxScale;

	public int numClouds;

	private new Transform transform;

	void Awake()
	{
		transform = GetComponent<Transform>();
	}

	void Start()
	{
		Vector3 baseOffset = Vector3.forward;
		Quaternion rotation = Quaternion.Euler( 0.0f, cloudIncrement, 0.0f );
		// generate a ring of the cloud prefabs
		for ( int count = 0; count < numClouds; ++count )
		{
			// rotate base offset and calculate full offset
			baseOffset = rotation * baseOffset;
			Vector3 cloudOffset = baseOffset * ( radius + Random.Range( 0, radiusError ) )
				+ Vector3.up * Random.Range( 0.0f, height );

			Transform cloud = ( GameObject.Instantiate(
				cloudPrefab,
				cloudOffset ,
				Quaternion.identity ) as GameObject ).GetComponent<Transform>();
			cloud.localScale = new Vector3( Random.Range( minScale, maxScale ),
			                                Random.Range( minScale, maxScale ),
			                                Random.Range( minScale, maxScale ) );
			cloud.SetParent( transform, false );
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(GetComponent<Transform>().position + Vector3.up * .5f, "S.png", true);
	}
}
