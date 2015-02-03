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
		
		loopObjHolder = new GameObject( string.Concat( "AudioPool", "_Loops" ) );
		loopObjHolder.transform.parent = soundManagerObj.transform;
	}
	
	public AudioSource CreateAudioObj()
	{
		GameObject audioSourceObj = new GameObject( string.Concat( "AudioPool", "_", audioObjs.Count.ToString() ), typeof( AudioSource ) );
		audioSourceObj.transform.parent = audioObjHolder.transform;
		
		audioObjs.Add( audioSourceObj.audio );
		audioSourceObj.audio.playOnAwake = false;
		
		return audioSourceObj.audio;
	}

	public AudioSource Play3DSoundAtPosition( AudioSource sourceData, Vector3 position )
    {
		foreach ( AudioSource audioSource in audioObjs )
		{
			Debug.Log( audioSource );
			if ( !audioSource.isPlaying )
			{
		        audioSource.transform.position = position;
				return PlayAudioObj( sourceData, audioSource );
			}
		}

		AudioSource newSource = CreateAudioObj();
		newSource.transform.position = position;
		audioObjs.Add( newSource );

		return PlayAudioObj( sourceData, newSource );
    }

	public AudioSource Play3DSoundAndFollow( AudioSource sourceData, Transform target )
    {
		foreach ( AudioSource audioSource in audioObjs )
		{
			Debug.Log( audioSource );
			if ( !audioSource.isPlaying )
			{
				audioSource.transform.position = target.position;
				audioSource.transform.parent = target;
				return PlayAudioObj( sourceData, audioSource );
			}
		}

		AudioSource newSource = CreateAudioObj();
		newSource.transform.position = target.position;
		newSource.transform.parent = target;
		audioObjs.Add( newSource );

		return PlayAudioObj( sourceData, newSource );
    }

	public AudioSource Play2DSound( AudioSource sourceData )
    {
		foreach ( AudioSource audioSource in audioObjs )
		{
			Debug.Log( audioSource );
			if ( !audioSource.isPlaying )
			{
				return PlayAudioObj( sourceData, audioSource );
			}
		}
		
		AudioSource newSource = CreateAudioObj();
		audioObjs.Add( newSource );

		return PlayAudioObj( sourceData, newSource );
    }

	AudioSource PlayAudioObj( AudioSource sourceData, AudioSource source)
	{
		source.GetCopyOf(sourceData);
		source.Play();

        if ( !source.loop )
		{
			StartCoroutine( ReturnSourceToPool( source, audioObjHolder.transform ) );
		}

        return source;
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