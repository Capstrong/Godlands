﻿using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Collider ) )]
public class Cuttable : MonoBehaviour
{
	[SerializeField] float _startingHealth = 0.0f;
	[SerializeField] ParticleSystem _hitParticle = null;
	[SerializeField] ParticleSystem _deathParticle = null;
	[SerializeField] float _particleLifetime = 0.0f;
	[SerializeField] Color _particleColor = Color.white;
	[SerializeField] AudioSource _hitSound = null;
	[SerializeField] AudioSource _deathSound = null;
	[SerializeField] float _verticalOffset = 0.0f;
	[SerializeField] int _maxNumberOfSwipes = 0;
	[SerializeField] float _respawnTime = 0.0f;
	[SerializeField] LayerMask _playerLayer = 0;
	[SerializeField] MinMaxF _alphaCutoffRange = new MinMaxF();
	[SerializeField] Color _startingOverlayColor = Color.white;
	[SerializeField] Color _endingOverlayColor = Color.white;

	[ReadOnly]
	private float _health = 0.0f;
	private bool _deactivated = false;
	private bool _readyToReactivate = false;
	private bool _isPlayerWithinTrigger = false;

	private Renderer _renderer;
	private Collider _collider;

	void Awake()
	{
		_renderer = GetComponent<Renderer>();
		_collider = GetComponent<Collider>();
	}

	void Start()
	{
		Reactivate();
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

			_renderer.material.SetFloat( "_Cutoff", Mathf.Lerp( _alphaCutoffRange.max, _alphaCutoffRange.min, _health / _startingHealth ) );
			_renderer.material.SetColor( "_ColorOverlayA", Color.Lerp( _endingOverlayColor, _startingOverlayColor, _health / _startingHealth ) );

			if ( _health <= 0 )
			{
				Deactivate();
			}
		}
	}

	void Deactivate()
	{
		ParticleSystem particleObj = (ParticleSystem) Instantiate( _deathParticle,
		                                                           transform.position + new Vector3( 0, _verticalOffset, 0 ),
		                                                           Quaternion.identity );
		particleObj.startColor = _particleColor;
		Destroy( particleObj.gameObject, _particleLifetime );

		SoundManager.Play3DSoundAtPosition( _deathSound, transform.position );

		Invoke( "ReadyToReactivate", _respawnTime );

		_deactivated = true;
		_renderer.enabled = false;
		_collider.isTrigger = true;
		_readyToReactivate = false;
	}

	void ReadyToReactivate()
	{
		_readyToReactivate = true;
	}

	void FixedUpdate()
	{
		if ( _deactivated && _readyToReactivate && !_isPlayerWithinTrigger)
		{
			Reactivate();
		}

		_isPlayerWithinTrigger = false;
	}

	void OnTriggerStay( Collider collider )
	{
		// TODO Wrap layermasks in a wrapper for safety
		if ( _playerLayer.Contains( collider.gameObject ) )
		{
			_isPlayerWithinTrigger = true;
		}
	}

	void Reactivate()
	{
		_deactivated = false;
		_renderer.enabled = true;
		_collider.isTrigger = false;
		_readyToReactivate = false;
		_health = _startingHealth;
		_renderer.material.SetFloat( "_Cutoff", _alphaCutoffRange.min );
		_renderer.material.SetColor( "_ColorOverlayA", _startingOverlayColor );
	}
}