using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : SingletonBehaviour<BackgroundMusicManager>
{
	[Tooltip( "Audio Source with settings for playing any background music" )]
	[SerializeField] AudioSource _audioSourceData = null;

	[ReadOnly]
	[SerializeField] AudioClip _currentAudioClip = null;
	[ReadOnly]
	[SerializeField] AudioSource _currentAudioSource = null;

	// Use this for initialization
	void Start () {
	
	}

	public static void PlayMusic( AudioClip newMusic )
	{
		instance._PlayMusic( newMusic );
	}

	void _PlayMusic( AudioClip newMusic )
	{
		if ( _currentAudioSource )
		{
			_currentAudioSource.Stop();
		}
		
		_audioSourceData.clip = newMusic;
		_currentAudioSource = SoundManager.Play2DSound( _audioSourceData );
	}
}
