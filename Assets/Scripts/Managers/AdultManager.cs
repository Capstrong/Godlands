using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdultManager : SingletonBehaviour<AdultManager>
{
	[SerializeField] GameObject _adultPrefab = null;

	[Range( 0.0f, 1.0f )]
	[SerializeField] float _goodBadCutoff = 0.5f;

	private List<AdultSpawnTag> _goodBuddySpawnPoints = new List<AdultSpawnTag>();
	private List<ResourceSpawner> _resourceSpawners = new List<ResourceSpawner>();

	void Start()
	{
		// Populate list of spawn points.
		_goodBuddySpawnPoints.AddRange( GameObject.FindObjectsOfType<AdultSpawnTag>() );
		_resourceSpawners.AddRange( GameObject.FindObjectsOfType<ResourceSpawner>() );
	}

	public static void SpawnAdult( BuddyStats buddyStats )
	{
		instance._SpawnAdult( buddyStats );
	}

	private void _SpawnAdult( BuddyStats buddyStats )
	{
		if ( buddyStats.happiness > _goodBadCutoff )
		{
			// Spawn good buddy.
			AdultSpawnTag spawnTag = _goodBuddySpawnPoints[Random.Range( 0, _goodBuddySpawnPoints.Count )];
			_goodBuddySpawnPoints.Remove( spawnTag );
			Transform spawnTransform = spawnTag.GetComponent<Transform>();
			GameObject newBuddy = (GameObject)Instantiate( _adultPrefab, spawnTransform.position, spawnTransform.rotation );

			//CopyBuddy( newBuddy.GetComponentInChildren<SkinnedMeshRenderer>(), buddyStats.GetComponentInChildren<SkinnedMeshRenderer>() );
		}
		else
		{
			// Spawn bad buddy.
			int index = Random.Range( 0, _resourceSpawners.Count );
			ResourceSpawner resourceSpawner = _resourceSpawners[index];
			_resourceSpawners.RemoveAt( index );

			// Deactive spawner once bad buddy is sitting on it.
			resourceSpawner.enabled = false;

			// Spawn bad buddy at the location of the resource spawner.
			Transform spawnTransform = resourceSpawner.GetComponent<Transform>();
			GameObject newBuddy = (GameObject)Instantiate( _adultPrefab, spawnTransform.position, Quaternion.identity );

			//CopyBuddy( newBuddy.GetComponentInChildren<SkinnedMeshRenderer>(), buddyStats.GetComponentInChildren<SkinnedMeshRenderer>() );
		}
	}

	// TODO: This is a duplicate of the method in BackBuddy. Find some way to not have to duplicate this code.
	void CopyBuddy( SkinnedMeshRenderer destBuddyMesh, SkinnedMeshRenderer sourceBuddyMesh )
	{
		MinMaxI copyBlendShapeIndicesRange = new MinMaxI( 3, 12 );

		for( int i = copyBlendShapeIndicesRange.min; i <= copyBlendShapeIndicesRange.max; i++ )
		{
			destBuddyMesh.SetBlendShapeWeight( i, sourceBuddyMesh.GetBlendShapeWeight( i ) );
		}

		destBuddyMesh.material.SetColor( "_TintColor1", sourceBuddyMesh.material.GetColor( "_TintColor1" ) );
		destBuddyMesh.material.SetColor( "_TintColor2", sourceBuddyMesh.material.GetColor( "_TintColor2" ) );
		
		destBuddyMesh.material.SetTexture( "_SkinTex", sourceBuddyMesh.material.GetTexture( "_SkinTex" ) );
	}
}
