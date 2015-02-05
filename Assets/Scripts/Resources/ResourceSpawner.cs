using UnityEngine;
using System.Collections;

public class ResourceSpawner : MonoBehaviour
{
	public ResourceHolder resourceHolderPrefab;
	public GameObject resourcePrefab;

	float groundCheckDist = 3f;

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

			RaycastHit hit = WadeUtils.RaycastAndGetInfo( new Ray( position + Vector3.up, Vector3.down ), groundCheckDist );
			if( hit.transform && resourcePrefab.GetComponent<Collider>() )
			{
				// change position to ray hit pos with offset depending on resource size
				position = hit.point + hit.normal * resourcePrefab.collider.bounds.size.y;
			}

			Instantiate( resourceHolderPrefab, position, Quaternion.identity );
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position + Vector3.up * .5f, "ResourceSpawner.png", true);
	}
}
