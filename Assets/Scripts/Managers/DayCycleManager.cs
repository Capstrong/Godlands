using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayCycleManager : SingletonBehaviour<DayCycleManager> 
{
	public delegate void EndOfDayCallback();

	public static float dayCycleLength { get { return instance._dayCycleLength; } }

	[Tooltip( "0 is midnight. 1/2 of Day Cycle Length is noon." ), SerializeField]
	float _currentTime = 0f;

	[Tooltip( "The length (in seconds) of a day" )]
	[SerializeField] float _dayCycleLength = 300.0f;

	[Range( 0.0f, 1.0f ), Tooltip( "The percent through the day that the morning starts." )]
	[SerializeField] float _dayStartTime = .1f;
	float morningTime
	{
		get
		{
			return _dayCycleLength * _dayStartTime;
		}
	}

	[Tooltip( "The duration, in seconds, of the midnight fade out effect." )]
	[SerializeField] float _blackoutDuration = 5.0f;

	[Tooltip( "The duration, in seconds, of the morning fade in effect." )]
	[SerializeField] float _lightupDuration = 5.0f;

	public static float currentTime { get { return instance._currentTime; } }

	[SerializeField] Image _midnightOverlay = null;

	EndOfDayCallback _endOfDayCallback = delegate() { };

	float timeUntilBlackout
	{
		get { return ( _dayCycleLength - _blackoutDuration ) - _currentTime; }
	}

	[Space( 10 ), Header( "Debug" )]
	[Tooltip( "Disable the midnight overlay and reseting the player's position." )]
	[SerializeField] bool _disableMidnight = false;

	void Start()
	{
		if ( !_disableMidnight )
		{
			MorningReset();
		}
	}

	void OnValidate()
	{
		CancelInvoke( "StartMidnightOverlay" );
		Invoke( "StartMidnightOverlay", timeUntilBlackout );
	}

	void Update ()
	{
		_currentTime = ( _currentTime + Time.deltaTime ) % dayCycleLength;
	}

	void MorningReset()
	{
		_currentTime = morningTime;
		StartCoroutine( FadeOutMidnightOverlay() );
		Invoke( "StartMidnightOverlay", timeUntilBlackout );
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
		StartCoroutine( FadeInMidnightOverlay() );
	}

	/**
	 * Used to fade to black at midnight.
	 */
	IEnumerator FadeInMidnightOverlay()
	{
		float elapsedTime = 0.0f;
		while ( true )
		{
			if ( elapsedTime < _blackoutDuration )
			{
				elapsedTime += Time.deltaTime;
				Color overlayColor = _midnightOverlay.color;
				overlayColor.a = elapsedTime / _blackoutDuration;
				_midnightOverlay.color = overlayColor;
				yield return null;
			}
			else
			{
				MorningReset();
				_endOfDayCallback();
				break;
			}
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
			if ( elapsedTime < _lightupDuration )
			{
				elapsedTime += Time.deltaTime;
				Color overlayColor = _midnightOverlay.color;
				overlayColor.a = 1.0f - ( elapsedTime / _lightupDuration );
				_midnightOverlay.color = overlayColor;
				yield return null;
			}
			else
			{
				break;
			}
		}
	}
}
