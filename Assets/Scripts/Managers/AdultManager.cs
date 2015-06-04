using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdultManager : SingletonBehaviour<AdultManager>
{
	[SerializeField] GameObject _adultPrefab = null;
	[SerializeField] TextVolume _adultTextVolume = null;

	[Range( 0.0f, 1.0f )]
	[SerializeField] float _goodBadCutoff = 0.5f;

	private List<AdultSpawnTag> _goodBuddySpawnPoints = new List<AdultSpawnTag>();
	private List<ResourceSpawner> _resourceSpawners = new List<ResourceSpawner>();

	[Tooltip( "The number of adult buddies at which point the game ends." )]
	[SerializeField] int _numGoalBuddies = 0;

	[SerializeField] AudioSource _goodBuddySong = null;
	[SerializeField] AudioSource _badBuddySong = null;

	[ReadOnly]
	[SerializeField] int _numGoodBuddies = 0;
	[ReadOnly]
	[SerializeField] int _numBadBuddies = 0;

	int numTotalBuddies
	{
		get { return _numBadBuddies + _numGoodBuddies; }
	}

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
		// Make sure the text volume is active.
		_adultTextVolume.gameObject.SetActive( true );

		if ( buddyStats.happiness > _goodBadCutoff )
		{
			// Spawn good buddy.
			AdultSpawnTag spawnTag = _goodBuddySpawnPoints[Random.Range( 0, _goodBuddySpawnPoints.Count )];
			_goodBuddySpawnPoints.Remove( spawnTag );
			Transform spawnTransform = spawnTag.GetComponent<Transform>();
			GameObject newBuddy = (GameObject)Instantiate( _adultPrefab, spawnTransform.position, spawnTransform.rotation );

			BuddyShaper.CopyBuddy( newBuddy.GetComponentInChildren<SkinnedMeshRenderer>(), buddyStats.bodyRenderer );

			SoundManager.Play2DSound( _goodBuddySong );

			_numGoodBuddies++;
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

			BuddyShaper.CopyBuddy( newBuddy.GetComponentInChildren<SkinnedMeshRenderer>(), buddyStats.bodyRenderer );

			SoundManager.Play2DSound( _badBuddySong );

			_numBadBuddies++;
		}

		buddyStats.BecomeAdult();

		CheckForEnding();
	}

	void CheckForEnding()
	{
		if ( numTotalBuddies >= _numGoalBuddies )
		{
			if ( _numGoodBuddies >= _numBadBuddies )
			{
				LevelUtils.LoadLevel( Level.GoodEnding );
			}
			else
			{
				LevelUtils.LoadLevel( Level.BadEnding );
			}
		}
	}
}
