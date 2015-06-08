using UnityEngine;
using System.Collections;

public class BuddyFaceController : MonoBehaviour 
{
	Animator _animator = null;

	void Awake()
	{
		_animator = GetComponentInParent<Animator>();
	}

	public void PlayEvent( string eventName )
	{
		// Second param here is the animationLayer to play an event on
		// 0 is the default layer, 1 is the face layer
		_animator.Play( eventName, 1 );
	}
}
