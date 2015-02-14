using UnityEngine;
using System.Collections;

public class ResourceSpawner : MonoBehaviour
{
	public ResourceHolder resourceHolderPrefab;
	public GameObject resourcePrefab;

	float groundCheckDist = 3f;

	public int total;
	public float radius;

	private new Transform transform = null;

	void Start()
	{
		// configure resource holder for the current resource type
		resourceHolderPrefab.resource = resourcePrefab;

		transform = GetComponent<Transform>();

		// spawn resource holders
		for ( int i = 0; i < total; i++ )
		{
			Vector3 offset = Quaternion.Euler( -90.0f, 0.0f, 0.0f ) * (Vector3) Random.insideUnitCircle;
			Vector3 spawnPosition = transform.position + offset * radius;

			RaycastHit hit = WadeUtils.RaycastAndGetInfo( new Ray( spawnPosition + Vector3.up, Vector3.down ), groundCheckDist );
			if( hit.transform && resourcePrefab.GetComponent<Collider>() )
			{
				// change position to ray hit pos with offset depending on resource size
				spawnPosition = hit.point + hit.normal * resourcePrefab.collider.bounds.size.y;
			}

			ResourceHolder resourceHolderInstance = (ResourceHolder) Instantiate( resourceHolderPrefab, spawnPosition, Quaternion.identity );
			resourceHolderInstance.gameObject.transform.parent = transform;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon( GetComponent<Transform>().position + Vector3.up * .5f, "ResourceSpawner.png", true );
	}
}
