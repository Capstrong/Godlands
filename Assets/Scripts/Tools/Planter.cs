using UnityEngine;
using System.Collections;

public class Planter : MonoBehaviour
{
	public GameObject grassPrefab;
	public Vector3 bounds = Vector3.one;
	public int plantCount = 20;
	public MinMaxF minMaxLean;

	public void Plant()
	{
		if ( !grassPrefab )
		{
			Debug.LogError( "You must select a grass prefab before planting!" );
			return;
		}

		Transform transform = GetComponent<Transform>();

		// First clear out all existing grass objects.
		foreach ( Transform child in transform )
		{
			GameObject.DestroyImmediate( child.gameObject );
		}

		// Now plant new ones.
		for ( int count = 0; count < plantCount; ++count )
		{
			PlantSingle();
		}
	}

	void PlantSingle()
	{
		Transform transform = GetComponent<Transform>();
		transform.localScale = Vector3.one;

		for ( int resourceSpawnTries = 0; resourceSpawnTries < 100; resourceSpawnTries++ )
		{
			Vector3 offset = new Vector3(
				Random.Range( -bounds.x * 0.5f, bounds.x * 0.5f ),
				0.0f,
				Random.Range( -bounds.z * 0.5f, bounds.z * 0.5f ) );
			Vector3 spawnPosition = transform.position + offset;

			RaycastHit hit = WadeUtils.RaycastAndGetInfo( new Ray( spawnPosition + Vector3.up * bounds.y, Vector3.down ), bounds.y );
			if ( hit.transform )
			{
				// change position to ray hit pos with offset depending on resource size
				spawnPosition = hit.point;
				
				Transform child =
					(GameObject.Instantiate(
						grassPrefab,
						spawnPosition,
						Quaternion.Euler(
							Random.Range( minMaxLean.min, minMaxLean.max ),
							Random.Range( 0.0f, 360.0f ),
							0.0f )
						) as GameObject)
					.GetComponent<Transform>();
				child.parent = transform;

				return;
			}
		}
	}

	void OnDrawGizmos()
	{
		Color gizmoColor = Color.green;

		Gizmos.color = gizmoColor;
		Gizmos.DrawWireCube( Vector3.up * bounds.y * 0.5f, bounds );

		gizmoColor.a = 0.3f;
		Gizmos.color = gizmoColor;
		Gizmos.DrawCube( Vector3.up * bounds.y * 0.5f, bounds );
	}
}
