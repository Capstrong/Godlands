﻿using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerStats ) )]
public class Cutting : ActorComponent
{
	[SerializeField] LayerMask _cuttableLayer = 0;
	public LayerMask cuttableLayer
	{
		get { return _cuttableLayer; }
	}

	[SerializeField] GameObject _visualEffect = null;
	[SerializeField] Vector3 _visualOffset = Vector3.zero;
	[SerializeField] float _lookOverrideDuration = 0.5f;

	PlayerStats _actorStats = null;

	void Start ()
	{
		_actorStats = GetComponent<PlayerStats>();
	}

	/**
	 * Checks the given RaycastHit to see if a Cuttable was hit.
	 * 
	 * Returns true if a Cuttable was hit, false otherwise.
	 * 
	 * If a Cuttable was hit, this will perform the cut action
	 * and create the visual effect.
	 */
	public bool CutCheck( RaycastHit hitInfo )
	{
		if ( hitInfo.collider && !hitInfo.collider.isTrigger )
		{
			GameObject cuttableObj = hitInfo.collider.gameObject;

			Cuttable cuttableComponent = cuttableObj.GetComponent<Cuttable>();

			if ( cuttableComponent )
			{
				Cut( cuttableComponent );
				CreateVisualEffect();
				return true;
			}
		}

		return false;
	}

	void Cut( Cuttable cuttableComponent )
	{
		cuttableComponent.Cut( _actorStats.GetStatValue( Stat.Cutting ) );
	}

	void CreateVisualEffect()
	{
		Vector3 camForward = Camera.main.transform.forward;
		camForward.y = 0.0f;

		actor.physics.OverrideLook( camForward, _lookOverrideDuration );

		Vector3 rotatedOffset = Quaternion.LookRotation( camForward ) * _visualOffset;

		Instantiate( _visualEffect, transform.position + rotatedOffset, Quaternion.LookRotation( camForward ) );
	}
}