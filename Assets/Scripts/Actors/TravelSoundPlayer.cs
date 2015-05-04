using UnityEngine;
using System.Collections;

public class TravelSoundPlayer : ScriptableObject
{
	[SerializeField] AudioClip[] _travelSounds;
	public AudioClip[] travelSounds
	{
		get { return _travelSounds; }
	}

	public AudioClip GetRandomClip()
	{
		return _travelSounds[Random.Range( 0, _travelSounds.Length )];
	}
}
