using UnityEngine;
using System.Collections;

public class ResourceSpawner : MonoBehaviour
{
	public ResourceHolder resourceHolderPrefab;
	public Collider resourcePrefab;

	float groundCheckDist = 3f;

	public int total;
	public float radius;

	private static int maxResourceSpawnTries = 100;

	private new Transform transform = null;

	void Start()
	{
		// configure resource holder for the current resource type
		resourceHolderPrefab.resource = resourcePrefab.gameObject;

		transform = GetComponent<Transform>();

		// spawn resource holders
		for ( int i = 0; i < total; i++ )
		{
			Vector3 spawnPosition = new Vector3();
			Vector3 offset = new Vector3();
			RaycastHit hit = new RaycastHit();

			for ( int resourceSpawnTries = 0; resourceSpawnTries < maxResourceSpawnTries; resourceSpawnTries++ )
			{
				offset = Quaternion.Euler( -90.0f, 0.0f, 0.0f ) * (Vector3) Random.insideUnitCircle;
				spawnPosition = transform.position + offset * radius;

				hit = WadeUtils.RaycastAndGetInfo( new Ray( spawnPosition + Vector3.up, Vector3.down ), groundCheckDist );
				if ( !hit.transform )
				{
					// The ray did not hit anything and the position was floating out somewhere
					// Retry to generate a new position
					continue;
				}
				
				// change position to ray hit pos with offset depending on resource size
				spawnPosition = hit.point + hit.normal * resourcePrefab.collider.bounds.size.y;

				break;
			}

			if ( spawnPosition != Vector3.zero )
			{
				ResourceHolder resourceHolderInstance = (ResourceHolder) Instantiate( resourceHolderPrefab, spawnPosition, Quaternion.identity );
				resourceHolderInstance.gameObject.transform.parent = transform;
			}
			else
			{
				Debug.LogError( "Exhausted all tries to spawn resource from spawner " + gameObject.name
				              + " at " + transform.position + " Consider moving or deleting it");
			}
		}
	}

	void OnDrawGizmos()
	{
		Transform trans = GetComponent<Transform>();
		Gizmos.DrawIcon( trans.position + Vector3.up * 0.5f, "ResourceSpawner.png", true );
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere( trans.position, radius );
	}
}
