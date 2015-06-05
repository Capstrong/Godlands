using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayCycleManager : SingletonBehaviour<DayCycleManager>
{
	public delegate void EndOfDayCallback();

	public static float dayCycleLength { get { return instance._dayCycleLength; } }

	[Tooltip( "0 is midnight. 1/2 of Day Cycle Length is noon." ), SerializeField]
	float _currentTime = 0f;
	
	public static float currentTime { get { return instance._currentTime; } }

	[Tooltip( "The length (in seconds) of a day" )]
	[SerializeField] float _dayCycleLength = 300.0f;

	[Range( 0.0f, 1.0f ), Tooltip( "The percent through the day that the morning starts." )]
	[SerializeField] float _dayStartTime = .1f;
	public float morningTime
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

	[SerializeField] Image _midnightOverlay = null;

	EndOfDayCallback _endOfDayCallback = delegate() { };

	public float timeUntilBlackout
	{
		get { return ( _dayCycleLength - _blackoutDuration ) - _currentTime; }
	}

	[Space( 10 ), Header( "Debug" )]
	[Tooltip( "Disable the midnight overlay and reseting the player's position." )]
	[SerializeField] bool _disableMidnight = false;
	[SerializeField] KeyCode _midnightTriggerKey = KeyCode.M;

	private Coroutine _midnightCoroutine = null;

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

#if UNITY_EDITOR
		if ( Input.GetKey( _midnightTriggerKey ) && _midnightCoroutine == null )
		{
			_TriggerMidnight( 2.0f );
		}
#endif
	}

	void MorningReset()
	{
		_currentTime = morningTime;
		StartCoroutine( FadeOutMidnightOverlay() );

		if( _midnightOverlay )
		{
			CancelInvoke( "StartMidnightOverlay" );
			Invoke( "StartMidnightOverlay", timeUntilBlackout );
		}
	}

	public static void TriggerMidnight( float overlayTime )
	{
		instance._TriggerMidnight( overlayTime );
	}

	void _TriggerMidnight( float overlayTime )
	{
		CancelInvoke( "StartMidnightOverlay" );
		StartMidnightOverlay( overlayTime );
	}

	// This returns percentage [0 to 1] along day, with start offset removed
	public float GetNormalizedCurrentTime()
	{
		float startTime = _dayStartTime * _dayCycleLength;
		return (_currentTime - startTime)/(_dayCycleLength - startTime );
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
		StartMidnightOverlay( _blackoutDuration );
	}

	void StartMidnightOverlay( float overlayTime )
	{
		if( _midnightOverlay )
		{
			_midnightCoroutine = StartCoroutine( FadeInMidnightOverlay( overlayTime ) );
		}
	}

	/**
	 * Used to fade to black at midnight.
	 */
	IEnumerator FadeInMidnightOverlay( float overlayTime )
	{
		float elapsedTime = 0.0f;
		while ( true )
		{
			if ( elapsedTime < overlayTime )
			{
				elapsedTime += Time.deltaTime;
				Color overlayColor = _midnightOverlay.color;
				_midnightOverlay.color = _midnightOverlay.color.SetAlpha( elapsedTime / overlayTime);
				yield return null;
			}
			else
			{
				MorningReset();
				_endOfDayCallback();
				break;
			}
		}

		_midnightCoroutine = null;
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
