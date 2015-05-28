using UnityEngine;
using System.Collections;


/*
 * This class is intended to redirect method calls coming from animation events to
 * the appropriate component on the player. It is needed because the animation events
 * only look at components on the same GameObject as the animator and all our
 * gameplay scripts are on the parent
 */

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
