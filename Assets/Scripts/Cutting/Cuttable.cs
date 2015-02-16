using UnityEngine;
using System.Collections;

public class Cuttable : MonoBehaviour
{
	[SerializeField] float _startingHealth = 0.0f;
	[SerializeField] ParticleSystem _hitParticle = null;
	[SerializeField] ParticleSystem _deathParticle = null;
	[SerializeField] float _particleLifetime = 0.0f;
	[SerializeField] Color _particleColor;
	[SerializeField] AudioSource _hitSound = null;
	[SerializeField] AudioSource _deathSound = null;
	[SerializeField] float _verticalOffset = 0.0f;
	[SerializeField] int _maxNumberOfSwipes = 0;
	[SerializeField] float _respawnTime = 0.0f;

	private float _health = 0.0f;

	void OnEnable()
	{
		_health = _startingHealth;
	}

	public void Cut( float cuttingLevel )
	{
		if ( _health / cuttingLevel <= _maxNumberOfSwipes )
		{
			_health -= cuttingLevel;

			ParticleSystem particleObj = (ParticleSystem)Instantiate( _hitParticle,
			                                                          transform.position + new Vector3( 0, _verticalOffset, 0 ),
			                                                          Quaternion.identity );
			particleObj.startColor = _particleColor;
			Destroy( particleObj.gameObject, _particleLifetime );

			SoundManager.Play3DSoundAtPosition( _hitSound, transform.position );

			if ( _health <= 0 )
			{
				Deactivate();
			}
		}
	}

	void Deactivate()
	{
		ParticleSystem particleObj = (ParticleSystem)Instantiate( _deathParticle,
		                                                          transform.position + new Vector3( 0, _verticalOffset, 0 ),
		                                                          Quaternion.identity );
		particleObj.startColor = _particleColor;
		Destroy( particleObj.gameObject, _particleLifetime );

		SoundManager.Play3DSoundAtPosition( _deathSound, transform.position );

		Invoke( "Reactivate", _respawnTime );

		gameObject.SetActive( false );
	}

	void Reactivate()
	{
		gameObject.SetActive( true );
	}
}
