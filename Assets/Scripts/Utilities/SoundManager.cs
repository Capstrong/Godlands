using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AudioType
{
	Sound = 0,
	Song = 1
}

[System.Serializable]
public class AudioPool
{
	public string poolName; // Used to name holderObj in scene hierarchy
	public AudioType poolType;

	[HideInInspector] public GameObject audioObjHolder;
	[HideInInspector] public GameObject loopObjHolder;
	public List<AudioSource> audioObjs;

	public void SetupPool( GameObject soundManagerObj )
	{
		// Create object to hold sounders
		audioObjHolder = new GameObject( poolName );
		audioObjHolder.transform.parent = soundManagerObj.transform;

		loopObjHolder = new GameObject( string.Concat( poolName, "_Loops" ) );
		loopObjHolder.transform.parent = soundManagerObj.transform;
	}

	public AudioSource CreateAudioObj()
	{
		GameObject audioSourceObj = new GameObject( string.Concat( poolName, "_", audioObjs.Count.ToString() ), typeof( AudioSource ) );
		audioSourceObj.transform.parent = audioObjHolder.transform;

		audioObjs.Add( audioSourceObj.audio );
		audioSourceObj.audio.playOnAwake = false;
		
		return audioSourceObj.audio;
	}
}

public class SoundManager : SingletonBehaviour<SoundManager>
{
	[SerializeField] AudioPool audioPool;

    // Use this for initialization
    void Awake()
    {
		audioPool.SetupPool( gameObject );
    }

	public AudioSource Play3DSoundAtPosition( AudioSource sourceData, Vector3 position )
    {
		foreach ( AudioSource audioSource in audioPool.audioObjs )
		{
			if ( !audioSource.isPlaying )
			{
		        audioSource.transform.position = position;
				return PlayAudioObj( audioSource, sourceData );
			}
		}

		AudioSource newSource = audioPool.CreateAudioObj();
		newSource.transform.position = position;
		audioPool.audioObjs.Add( newSource );
		return PlayAudioObj( newSource, sourceData );
    }

	public AudioSource Play3DSoundAndFollow( AudioSource sourceData, Transform target )
    {
		foreach ( AudioSource audioSource in audioPool.audioObjs )
		{
			if ( !audioSource.isPlaying )
			{
				audioSource.transform.position = target.position;
				audioSource.transform.parent = target;
				return PlayAudioObj( audioSource, sourceData );
			}
		}

		AudioSource newSource = audioPool.CreateAudioObj();

        // Attach to target
		newSource.transform.position = target.position;
		newSource.transform.parent = target;
		audioPool.audioObjs.Add( newSource );

		// Set up audio source
		return PlayAudioObj( newSource, sourceData );
    }

    public AudioSource Play2DSound( AudioSource sourceData )
    {
		foreach ( AudioSource audioSource in audioPool.audioObjs )
		{
			if ( !audioSource.isPlaying )
			{
				return PlayAudioObj( sourceData, audioSource );
			}
		}
		
		AudioSource newSource = audioPool.CreateAudioObj();
		audioPool.audioObjs.Add( newSource );

		return PlayAudioObj( sourceData, newSource );
    }

	AudioSource PlayAudioObj( AudioSource sourceInfo, AudioSource source, bool loop = false )
	{
        // Set up audio source
		CopyAudioSource( sourceInfo, source );
		source.Play();

        // If this sound will loop forever, then replace it in the pool
        if ( loop )
        {
			source.name = string.Concat( source.name, "_Looping" );
        }
        else
		{
			StartCoroutine( ReturnSourceToPool( source, audioPool.audioObjHolder.transform ) );
		}

        return source;
    }

	void CopyAudioSource( AudioSource original, AudioSource destination )
	{
		System.Reflection.FieldInfo[] fields = typeof( AudioSource ).GetFields(); 
		foreach ( System.Reflection.FieldInfo field in fields )
		{
			field.SetValue( destination, field.GetValue( original ) );
		}

		destination.name = original.name;
	}
	
	void ResetSource( AudioSource source )
	{
		GameObject go = source.gameObject;
		Destroy( source );
		go.AddComponent<AudioSource>();
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