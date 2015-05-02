using UnityEngine;
using System.Collections;

public class AnimationEventRedirector: MonoBehaviour
{
	PlayerControls _playerControls = null;

	void Start()
	{
		_playerControls = GetComponentInParent<PlayerControls>();
	}

	public void PlayStepSound()
	{
		_playerControls.PlayStepSound();
	}
}
