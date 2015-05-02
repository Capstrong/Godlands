using UnityEngine;
using System.Collections;

public class AnimationEventRedirector: MonoBehaviour
{
	TravelSounds _travelSounds = null;

	void Start()
	{
		_travelSounds = GetComponentInParent<TravelSounds>();
	}

	public void PlayStepSound()
	{
		_travelSounds.PlayStepSound();
	}
}
