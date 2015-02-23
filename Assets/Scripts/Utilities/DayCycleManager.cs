using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayCycleManager : SingletonBehaviour<DayCycleManager> 
{
	public static float dayCycleLength { get { return instance._dayCycleLength; } }

	[Tooltip( "The length (in seconds) of a day" )]
	[SerializeField] float _dayCycleLength = 60.0f;

	[SerializeField] float _dayStartTime = 0f;
	[SerializeField] float _blackOutStartTime = 0f;

	public static float dayCycleTimer { get { return instance._dayCycleTimer; } }

	[ReadOnly( "Day Cycle Timer" ), Tooltip( "0 is midnight. 1/2 of Day Cycle Length is noon." )]
	[SerializeField] float _dayCycleTimer = 0f;

	[SerializeField] PlayerControls _playerControls = null;

	[SerializeField] Image _midnightOverlay = null;

	bool _hasStartedMidnightOverlay = false;

	Coroutine _midnightOverlayCoroutine = null;

	void Start()
	{
		_dayCycleTimer = _dayStartTime;
	}

	// Update is called once per frame
	void Update () {
		_dayCycleTimer += Time.deltaTime;

		if ( !_hasStartedMidnightOverlay && _dayCycleTimer > _blackOutStartTime )
		{
			StartMidnightOverlay();
		}

		if ( _dayCycleTimer > _dayCycleLength )
		{
			BuddyManager.DecrementAllBuddyResources();
			_playerControls.Respawn();
			_dayCycleTimer = _dayCycleTimer - _dayCycleLength + _dayStartTime; // Take overflow into account
			EndMidnightOverlay();
		}
	}

	void StartMidnightOverlay()
	{
		_hasStartedMidnightOverlay = true;
		_midnightOverlayCoroutine = StartCoroutine( FadeInMidnightOverlay() );
	}

	void EndMidnightOverlay()
	{
		_hasStartedMidnightOverlay = false;

		StopCoroutine( _midnightOverlayCoroutine );

		Color color = _midnightOverlay.color;
		color.a = 0.0f;
		_midnightOverlay.color = color;
	}

	IEnumerator FadeInMidnightOverlay()
	{
		while ( true )
		{
			Color color = _midnightOverlay.color;
			color.a = ( _dayCycleTimer - _blackOutStartTime ) / ( _dayCycleLength - _blackOutStartTime );
			_midnightOverlay.color = color;
			yield return null;
		}


	}
}
