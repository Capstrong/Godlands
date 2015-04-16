using UnityEngine;
using System.Collections;

public class ResourceSpawner : MonoBehaviour
{
	public ResourceHolder resourceHolderPrefab;
	public Collider resourcePrefab;

	float _groundCheckDist = 7f;

	public int total;
	public float radius;

	private static int _maxResourceSpawnTries = 100;

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

			for ( int resourceSpawnTries = 0; resourceSpawnTries < _maxResourceSpawnTries; resourceSpawnTries++ )
			{
				offset = Quaternion.Euler( -90.0f, 0.0f, 0.0f ) * (Vector3) Random.insideUnitCircle;
				spawnPosition = transform.position + offset * radius;

				hit = WadeUtils.RaycastAndGetInfo( new Ray( spawnPosition + Vector3.up, Vector3.down ), _groundCheckDist );
				if ( !hit.transform )
				{
					// The ray did not hit anything and the position was floating out somewhere
					// Retry to generate a new position
					continue;
				}
				
				// change position to ray hit pos with offset depending on resource size
				spawnPosition = hit.point + hit.normal * resourcePrefab.bounds.size.y;

				break;
			}

			if ( spawnPosition != Vector3.zero )
			{
				Quaternion spawnRotation = Quaternion.Euler( 0f, Random.Range( -180f, 180f ) , 0f );
				ResourceHolder resourceHolderInstance = (ResourceHolder) Instantiate( resourceHolderPrefab, spawnPosition, spawnRotation );
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

		Color gizmoColor = Color.yellow;

		Gizmos.color = gizmoColor;
		Gizmos.DrawWireCube( trans.position, Vector3.one * radius * 2f );

		gizmoColor.a = 0.3f;
		Gizmos.color = gizmoColor;
		Gizmos.DrawCube( trans.position, Vector3.one * radius * 2f );
	}
}
