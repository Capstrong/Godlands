using UnityEngine;
using System.Collections;

public class BuddyFaceController : MonoBehaviour 
{
	Animator _animator = null;
	Animator animator
	{
		get 
		{
			if( !_animator )
			{
				_animator = GetComponentInParent<Animator>();
			}
			
			return _animator;
		}
	}
	void Awake()
	{

	}

	public void PlayEvent( string eventName )
	{
		// Second param here is the animationLayer to play an event on
		// 0 is the default layer, 1 is the face layer
		animator.Play( eventName, 1 );
	}
}
