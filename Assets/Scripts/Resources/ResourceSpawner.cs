using UnityEngine;
using System.Collections;

public class ResourceSpawner : MonoBehaviour
{
	public ResourceHolder resourceHolderPrefab;
	public GameObject resourcePrefab;

	float groundCheckDist = 3f;

	public int total;
	public float radius;

	private static int maxResourceSpawnTries = 100;

	private new Transform transform = null;

	void Start()
	{
		// configure resource holder for the current resource type
		resourceHolderPrefab.resource = resourcePrefab;

		transform = GetComponent<Transform>();

		if ( !resourcePrefab.GetComponent<Collider>() )
		{
			Debug.LogError( "Resource prefab " + resourcePrefab + " does not have a collider. Halting resource spawning." );
			return;
		}

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

				/*
				Vector3 reallyHighUpSpot = spawnPosition + Vector3.up * WadeUtils.LARGENUMBER;

				// Do a raycast from really high up and exclude the renderzone layer
				hit = WadeUtils.RaycastAndGetInfo( new Ray( reallyHighUpSpot, Vector3.down ), ~(1 << 13), WadeUtils.LARGENUMBER );

				if ( hit.collider && hit.point.y > spawnPosition.y )
				{
					// This is supposed to check if the spawn point is within another object but doesn't entirely work
					continue;
				}*/

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
		Gizmos.DrawIcon( GetComponent<Transform>().position + Vector3.up * .5f, "ResourceSpawner.png", true );
	}
}
