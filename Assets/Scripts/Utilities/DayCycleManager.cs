using UnityEngine;
using System.Collections;

public class DayCycleManager : SingletonBehaviour<DayCycleManager> 
{
	public static float dayCycleLength { get { return instance._dayCycleLength; } }

	[Tooltip( "The length (in seconds) of a day" )]
	[SerializeField] float _dayCycleLength = 60;

	public static float dayCycleTimer { get { return instance._dayCycleTimer; } }

	[ReadOnly( "Day Cycle Timer" )]
	[SerializeField] float _dayCycleTimer = 0f;

	[SerializeField] PlayerControls _playerControls = null;

	void Start()
	{
		StartCoroutine( MidnightTriggerRoutine() );
	}

	// Update is called once per frame
	void Update () {
		_dayCycleTimer += Time.deltaTime;

		if ( _dayCycleTimer > _dayCycleLength )
		{
			_dayCycleTimer -= _dayCycleLength;
		}
	}

	IEnumerator MidnightTriggerRoutine()
	{
		// Starts at noon, wait for midnight
		yield return new WaitForSeconds( _dayCycleLength * 0.5f );

		while ( true )
		{
			BuddyManager.DecrementAllBuddyResources();
			yield return new WaitForSeconds( _dayCycleLength );
		}
	}
}
