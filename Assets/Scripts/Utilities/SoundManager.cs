using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : SingletonBehaviour<SoundManager>
{
	[HideInInspector] public GameObject audioObjHolder = null;
	[HideInInspector] public GameObject loopObjHolder = null;
	public List<AudioSource> audioObjs = new List<AudioSource>();

	AudioSource defaultSource;

    void Awake()
    {
		defaultSource = gameObject.AddComponent<AudioSource>();
		defaultSource.playOnAwake = false;

		SetupPool( gameObject );
    }

	public void SetupPool( GameObject soundManagerObj )
	{
		// Create object to hold sounders
		audioObjHolder = new GameObject( "AudioPool" );
		audioObjHolder.transform.parent = soundManagerObj.transform;
		
		loopObjHolder = new GameObject( "AudioPool_Loops" );
		loopObjHolder.transform.parent = soundManagerObj.transform;
	}
	
	public AudioSource CreateAudioObj()
	{
		GameObject audioSourceObj = new GameObject( "AudioPool_" + audioObjs.Count.ToString(), typeof( AudioSource ) );
		audioSourceObj.transform.parent = audioObjHolder.transform;
		
		audioObjs.Add( audioSourceObj.audio );
		audioSourceObj.audio.playOnAwake = false;
		
		return audioSourceObj.audio;
	}

	public AudioSource Play3DSoundAtPosition( AudioSource sourceData, Vector3 position )
    {
		AudioSource source = GetSource();

		source.transform.position = position;
		PlayAudioObj( sourceData, source );
		audioObjs.Add( source );

		return source;
    }

	public AudioSource Play3DSoundAndFollow( AudioSource sourceData, Transform target )
    {
		AudioSource source = GetSource();

		source.transform.position = target.position;
		source.transform.parent = target;
		PlayAudioObj( sourceData, source );
		audioObjs.Add( source );

		return source;
    }

	public AudioSource Play2DSound( AudioSource sourceData )
    {
		AudioSource source = GetSource();

		PlayAudioObj( sourceData, source );
		audioObjs.Add( source );

		return source;
    }

	AudioSource GetSource()
	{
		AudioSource returnSource = null;

		foreach ( AudioSource audioSource in audioObjs )
		{
			if ( !audioSource.isPlaying )
			{
				returnSource = audioSource;
			}
		}

		if(!returnSource)
		{
			returnSource = CreateAudioObj();
		}

		return returnSource;
	}

	void PlayAudioObj( AudioSource sourceData, AudioSource source)
	{
		source.MakeCopyOf(sourceData);
		source.Play();

        if ( !source.loop )
		{
			StartCoroutine( ReturnSourceToPool( source, audioObjHolder.transform ) );
		}
    }
	
	void ResetSource( AudioSource source )
	{
		source.GetCopyOf( defaultSource );
	}

    IEnumerator ReturnSourceToPool( AudioSource audioSource, Transform parent )
    {
		yield return new WaitForSeconds( audioSource.clip.length );

		if( !audioSource.loop )
		{
			ResetSource( audioSource );
	        audioSource.transform.position = parent.position;
			audioSource.transform.parent = parent;
		}
    }
}