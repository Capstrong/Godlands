using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SounderTag : MonoBehaviour {}
public class SongTag : MonoBehaviour {}

public enum AudioType
{
	Sound = 0,
	Song = 1
}

[System.Serializable]
public class AudioPool
{
	public string poolName;
	public AudioType poolType;
	public AudioClip[] audioClips;
	
	[HideInInspector] public GameObject audioObjHolder;
	[HideInInspector] public List<AudioSource> audioObjs;

	public Dictionary<string, AudioClip> clipList = new Dictionary<string, AudioClip>();
	public bool muted = false;

	public void SetMuted(bool mute)
	{
		muted = mute;
	}

	public void SetupPool(GameObject soundManagerObj)
	{
		// Create object to hold sounders
		audioObjHolder = new GameObject(poolName);
		audioObjHolder.transform.parent = soundManagerObj.transform;
	}

	public AudioSource CreateAudioObj()
	{
		GameObject audioSourceObj = new GameObject(poolName, typeof(AudioSource));
		audioSourceObj.transform.parent = audioObjHolder.transform;

		audioObjs.Add(audioSourceObj.audio);
		audioSourceObj.audio.playOnAwake = false;
		
		return audioSourceObj.audio;
	}
}

public class SoundManager : SingletonBehaviour<SoundManager>
{
	[SerializeField] AudioPool[] audioPools = null;

    // Use this for initialization
    void Awake()
    {
		foreach(AudioPool audioPool in audioPools)
		{
			foreach(AudioClip audioClip in audioPool.audioClips)
			{
				audioPool.clipList.Add(audioClip.name, audioClip);
			}

			audioPool.SetupPool(gameObject);
		}
    }

	public AudioSource PlaySoundAtPosition(string clip, Vector3 position)
    {
		foreach(AudioPool audioPool in audioPools)
		{
			if(audioPool.poolType == AudioType.Sound)
			{
				foreach(AudioSource audioSource in audioPool.audioObjs)
				{
					if(!audioSource.isPlaying)
					{
				        audioSource.transform.position = position;
						return PlayAudioObj(audioSource, clip);
					}
				}

				AudioSource newSource = audioPool.CreateAudioObj();
				newSource.transform.position = position;
				audioPool.audioObjs.Add(newSource);
				return PlayAudioObj(newSource, clip);
			}
		}

		return null;
    }

	public AudioSource PlaySoundAndFollow(string clip, Transform target)
    {
		foreach(AudioPool audioPool in audioPools)
		{
			if(audioPool.poolType == AudioType.Sound)
			{
				foreach(AudioSource audioSource in audioPool.audioObjs)
				{
					if(!audioSource.isPlaying)
					{
						audioSource.transform.position = target.position;
						audioSource.transform.parent = target;
						return PlayAudioObj(audioSource, clip);
					}
				}

				AudioSource newSource = audioPool.CreateAudioObj();

		        // Attach to target
				newSource.transform.position = target.position;
				newSource.transform.parent = target;
				audioPool.audioObjs.Add(newSource);

				// Set up audio source
				return PlayAudioObj(newSource, clip);
			}
		}

		return null;
    }

    public AudioSource Play2DSong(string clip)
    {
		foreach(AudioPool audioPool in audioPools)
		{
			if(audioPool.poolType == AudioType.Song)
			{
				foreach(AudioSource audioSource in audioPool.audioObjs)
				{
					if(!audioSource.isPlaying)
					{
						return PlayAudioObj(audioSource, clip);
					}
				}
				
				AudioSource newSource = audioPool.CreateAudioObj();
				audioPool.audioObjs.Add(newSource);

				return PlayAudioObj(newSource, clip);
			}
		}

		return null;
    }

	public AudioSource PlaySongAtPosition(string clip, Vector3 position)
    {
		foreach(AudioPool audioPool in audioPools)
		{
			if(audioPool.poolType == AudioType.Sound)
			{				
				foreach(AudioSource audioSource in audioPool.audioObjs)
				{
					if(!audioSource.isPlaying)
					{
						audioSource.transform.position = position;
						return PlayAudioObj(audioSource, clip);
					}
				}
				
				AudioSource newSource = audioPool.CreateAudioObj();
				newSource.transform.position = position;
				audioPool.audioObjs.Add(newSource);

				return PlayAudioObj(newSource, clip);
			}
		}
		
		return null;
    }

	public AudioSource PlaySongAndFollow(string clip, Transform target)
    {
		foreach(AudioPool audioPool in audioPools)
		{
			if(audioPool.poolType == AudioType.Sound)
			{
				foreach(AudioSource audioSource in audioPool.audioObjs)
				{
					if(!audioSource.isPlaying)
					{
						audioSource.transform.position = target.position;
						audioSource.transform.parent = target;
						return PlayAudioObj(audioSource, clip);
					}
				}

				AudioSource newSource = audioPool.CreateAudioObj();
				newSource.transform.position = target.position;
				newSource.transform.parent = target;
				audioPool.audioObjs.Add(newSource);

				return PlayAudioObj(newSource, clip);
			}
		}

		return null;
	}

	AudioSource PlayAudioObj(AudioSource audioSource, string clip, bool loop = false)
	{
		foreach(AudioPool audioPool in audioPools)
		{
			if(audioPool.clipList.ContainsKey(clip))
			{
		        // Set up audio source
				audioSource.clip = audioPool.clipList[clip];
				audioSource.Play();

		        // If this sound will loop forever, then replace it in the pool
		        if (loop)
		        {
					audioSource.transform.parent = null;
					audioSource.name = "AudioLoop_" + clip;

					audioPool.audioObjs.Remove(audioSource);

					GameObject replacementObj = new GameObject("AudioObj", typeof(AudioSource));
					audioPool.audioObjs.Add(replacementObj.audio);

					replacementObj.audio.playOnAwake = false;
					replacementObj.transform.parent = audioPool.audioObjHolder.transform;
		        }
		        else
				{
		            StartCoroutine(ReturnSourceToPool(audioSource, audioPool.audioObjHolder.transform));
				}

		        return audioSource;
			}
		}

		return null;
    }
	
	void ResetSource(AudioSource source)
	{
		source.volume = 1f;
		source.pitch = 1f;
		source.loop = false;
	}

    IEnumerator ReturnSourceToPool(AudioSource audioSource, Transform parent)
    {
		yield return new WaitForSeconds(audioSource.clip.length);

		ResetSource(audioSource);
        audioSource.transform.position = parent.position;
		audioSource.transform.parent = parent;
    }
}
