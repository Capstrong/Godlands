using UnityEngine;
using System.Collections;

public class BackBuddy : MonoBehaviour 
{
	public BuddyStats hiddenBuddy; // The buddy that is currently on the back.

	[SerializeField] ParticleSystem _adultParticles = null;
	[SerializeField] ParticleSystem _tearsParticles = null;
	[SerializeField] ParticleSystem _heartParticles = null;
	[SerializeField] ParticleSystem _cheerParticles = null;

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
			_adultParticles.enableEmission = true;
			_adultParticles.Play();

			if ( buddyStats.isGoodAdult )
			{
				_cheerParticles.enableEmission = true;
				_cheerParticles.Play();
			}
			else
			{
				_tearsParticles.enableEmission = true;
				_tearsParticles.Play();
			}
		}
		else
		{
			_heartParticles.enableEmission = true;
			_heartParticles.Play();
		}
	}

	public void Reset()
	{
		_adultParticles.enableEmission = false;
		_adultParticles.Stop();

		_cheerParticles.enableEmission = false;
		_cheerParticles.Stop();

		_tearsParticles.enableEmission = false;
		_tearsParticles.Stop();

		_heartParticles.enableEmission = false;
		_heartParticles.Stop();

		hiddenBuddy.BackReset();
		hiddenBuddy = null;
	}
}
