using UnityEngine;
using System.Collections;

public class BackBuddy : MonoBehaviour 
{
	public BuddyStats hiddenBuddy; // The buddy that is currently on the back.

	ParticleSystem adultParticles;

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

	SkinnedMeshRenderer mySkinnedMesh = null;

	void Awake()
	{
		mySkinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();

		adultParticles = GetComponentInChildren<ParticleSystem>();
	}

	public void PlayEvent( string eventName )
	{
		// Second param here is the animationLayer to play an event on
		// 0 is the default layer, 1 is the face layer
		animator.Play( eventName, 1 );
	}

	public void CopyBuddy( BuddyStats buddyStats, SkinnedMeshRenderer sourceMesh )
	{
		hiddenBuddy = buddyStats;
		BuddyShaper.CopyBuddy( mySkinnedMesh, sourceMesh );

		if ( buddyStats.isOfAge )
		{
			adultParticles.enableEmission = true;
			adultParticles.Play();
		}
	}

	public void Reset()
	{
		adultParticles.Stop();
		hiddenBuddy.BackReset();
		hiddenBuddy = null;
	}
}
