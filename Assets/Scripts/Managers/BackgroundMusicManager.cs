using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : SingletonBehaviour<BackgroundMusicManager>
{
	[Tooltip( "Seconds it takes to fade out a song" )]
	[SerializeField] float fadeOutTime = 0f;
	[Tooltip( "Seconds it takes to fade in a song" )]
	[SerializeField] float fadeInTime = 0f;

	[ReadOnly]
	[SerializeField] AudioSource _currentAudioSource = null;

	Coroutine _fadeOutCoroutine = null;
	Coroutine _fadeInCoroutine = null;

	public static void PlayMusic( AudioSource newMusic )
	{
		instance._PlayMusic( newMusic );
	}

	void _PlayMusic( AudioSource newMusic )
	{
		if ( _currentAudioSource && newMusic.clip == _currentAudioSource.clip )
		{
			// Already playing that music
			return;
		}

		if ( _currentAudioSource )
		{
			if ( _fadeOutCoroutine != null )
			{
				StopCoroutine( _fadeOutCoroutine );
			}
			_fadeOutCoroutine = StartCoroutine( FadeOutSongRoutine( _currentAudioSource ) );
		}
		
		_currentAudioSource = SoundManager.Play2DSound( newMusic );

		if ( _fadeInCoroutine != null )
		{
			StopCoroutine( _fadeInCoroutine );
		}
		_fadeInCoroutine = StartCoroutine( FadeInSongRoutine( _currentAudioSource ) );
	}

	IEnumerator FadeOutSongRoutine( AudioSource audioSource )
	{
		float startTime = Time.time;
		float startVolume = audioSource.volume;

		while ( Time.time - startTime < fadeOutTime )
		{
			audioSource.volume = startVolume * ( fadeOutTime - ( Time.time - startTime ) ) / fadeOutTime;

			yield return null;
		}

		audioSource.Stop();
	}

	IEnumerator FadeInSongRoutine( AudioSource audioSource )
	{
		float startTime = Time.time;
		float startVolume = audioSource.volume;

		while ( Time.time - startTime < fadeInTime )
		{
			audioSource.volume = startVolume * ( Time.time - startTime ) / fadeInTime;

			yield return null;
		}

		audioSource.volume = startVolume;
	}
}
