using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayCycleManager : SingletonBehaviour<DayCycleManager> 
{
	public delegate void EndOfDayCallback();

	public static float dayCycleLength { get { return instance._dayCycleLength; } }

	[ReadOnly( "Time Of Day" ), Tooltip( "0 is midnight. 1/2 of Day Cycle Length is noon." ), SerializeField]
	float _dayCycleTimer = 0f;

	[Tooltip( "The length (in seconds) of a day" )]
	[SerializeField] float _dayCycleLength = 60.0f;

	[SerializeField] float _dayStartTime = 0f;
	[Tooltip( "The duration, in seconds, of the midnight fade out effect." )]
	[SerializeField] float _blackoutDuration = 5.0f;

	[Tooltip( "The duration, in seconds, of the morning fade in effect." )]
	[SerializeField] float _lightupDuration = 5.0f;

	public static float dayCycleTimer { get { return instance._dayCycleTimer; } }

	[SerializeField] Image _midnightOverlay = null;

	bool _hasStartedMidnightOverlay = false;

	Coroutine _midnightOverlayCoroutine = null;

	EndOfDayCallback _endOfDayCallback = delegate() { };

	void Start()
	{
		_dayCycleTimer = _dayStartTime;
	}

	// Update is called once per frame
	void Update ()
	{
		_dayCycleTimer += Time.deltaTime;

		if ( !_hasStartedMidnightOverlay && _dayCycleTimer > ( _dayCycleLength - _blackoutDuration ) )
		{
			StartMidnightOverlay();
		}

		if ( _dayCycleTimer > _dayCycleLength )
		{
			_dayCycleTimer = _dayStartTime;
			EndMidnightOverlay();
			StartMorningOverlay();

			_endOfDayCallback();
		}
	}

	public static void RegisterEndOfDayCallback( EndOfDayCallback callback )
	{
		instance._endOfDayCallback += callback;
	}

	public static void DeregisterEndOfDayCallback( EndOfDayCallback callback )
	{
		instance._endOfDayCallback -= callback;
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
		_midnightOverlayCoroutine = null;

		Color overlayColor = _midnightOverlay.color;
		_midnightOverlay.color = overlayColor;
	}

	void StartMorningOverlay()
	{
		StartCoroutine( FadeOutMidnightOverlay() );
	}

	/**
	 * Used to fade to black at midnight.
	 */
	IEnumerator FadeInMidnightOverlay()
	{
		float elapsedTime = 0.0f;
		while ( true )
		{
			elapsedTime += Time.deltaTime;
			Color overlayColor = _midnightOverlay.color;
			overlayColor.a = elapsedTime / _blackoutDuration;
			_midnightOverlay.color = overlayColor;
			yield return null;
		}
	}

	/**
	 * Used to fade back to light in the morning.
	 */
	IEnumerator FadeOutMidnightOverlay()
	{
		float elapsedTime = 0.0f;
		while ( true )
		{
			elapsedTime += Time.deltaTime;
			Color overlayColor = _midnightOverlay.color;
			overlayColor.a = 1.0f - ( elapsedTime / _lightupDuration );
			_midnightOverlay.color = overlayColor;

			if ( elapsedTime < _lightupDuration )
			{
				yield return null;
			}
			else
			{
				break;
			}
		}
	}
}
