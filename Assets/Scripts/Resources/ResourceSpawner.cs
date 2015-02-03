using UnityEngine;
using System.Collections;

public class ResourceSpawner : MonoBehaviour
{
	public ResourceHolder resourceHolderPrefab;
	public GameObject resourcePrefab;

	public int total;
	public float radius;

	void Start()
	{
		// configure resource holder for the current resource type
		resourceHolderPrefab.resource = resourcePrefab;

		// spawn resource holders
		for ( int i = 0; i < total; i++ )
		{
			Vector3 offset = Quaternion.Euler( -90.0f, 0.0f, 0.0f ) * (Vector3)Random.insideUnitCircle;
			Vector3 position = transform.position + offset * radius;

			Instantiate( resourceHolderPrefab, position, Quaternion.identity );
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon( transform.position + Vector3.up * .5f, "S.png", true );
	}
}
