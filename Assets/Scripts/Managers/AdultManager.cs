using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdultManager : SingletonBehaviour<AdultManager>
{
	[SerializeField]
	GameObject _adultPrefab = null;

	private List<AdultSpawnTag> _spawnPoints = new List<AdultSpawnTag>();

	void Start()
	{
		// Populate list of spawn points.
		_spawnPoints.AddRange( GameObject.FindObjectsOfType<AdultSpawnTag>() );
	}

	public static void SpawnAdult()
	{
		instance._SpawnAdult();
	}

	private void _SpawnAdult()
	{
		AdultSpawnTag spawnTag = _spawnPoints[Random.Range( 0, _spawnPoints.Count )];
		_spawnPoints.Remove( spawnTag );

		Transform spawnTransform = spawnTag.GetComponent<Transform>();
		Instantiate( _adultPrefab, spawnTransform.position, spawnTransform.rotation );
	}
}
