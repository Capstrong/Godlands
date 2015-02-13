﻿using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerActorStats ) )]
public class CuttingA : ActorComponent
{

	[SerializeField] LayerMask _cutableLayer = 0;
	[SerializeField] KeyCode _cuttingButton = KeyCode.J;
	[SerializeField] float _cuttingDistance = 0f;
	[SerializeField] GameObject _visualEffect = null;
	[SerializeField] Vector3 _visualOffset = Vector3.zero;

	PlayerActorStats actorStats = null;

	// Use this for initialization
	void Start ()
	{
		actorStats = GetComponent<PlayerActorStats>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ( Input.GetKeyDown( _cuttingButton ) )
		{
			Vector3 camForward = Camera.main.transform.forward;
			camForward.Set( camForward.x, 0, camForward.z );

			actor.actorPhysics.OverrideLook( camForward, 0.5f );

			Vector3 rotatedOffset = Quaternion.LookRotation( camForward ) * _visualOffset;

			Instantiate( _visualEffect, transform.position + rotatedOffset, Quaternion.LookRotation( camForward ) );

			RaycastHit hitInfo;
			Physics.Raycast( new Ray( transform.position, camForward ), out hitInfo, _cuttingDistance, _cutableLayer );

			if ( hitInfo.collider )
			{
				GameObject cutableObj = hitInfo.collider.gameObject;

				CutableA cutableComponent = cutableObj.GetComponent<CutableA>();

				if ( cutableComponent )
				{
					Cut( cutableComponent );
				}
				else
				{
					CutableB cutableComponentB = cutableObj.GetComponent<CutableB>();

					if ( cutableComponentB )
					{
						Cut( cutableComponentB );
					}
					else
					{
						CutableC cutableComponentC = cutableObj.GetComponent<CutableC>();

						if ( cutableComponentC )
						{
							Cut( cutableComponentC );
						}
						else
						{
							Debug.LogError( "Attach Cutable component to " + cutableObj.name + " at " + cutableObj.transform.position );
						}
					}
				}
			}
		}
	}

	void Cut( CutableA cutableComponent )
	{
		cutableComponent.Cut( actorStats.GetStatValue( Stat.Cutting ) );
	}

	void Cut ( CutableB cutableComponent )
	{
		cutableComponent.Cut( actorStats.GetStatValue( Stat.Cutting ) );
	}

	void Cut( CutableC cutableComponent )
	{
		cutableComponent.Cut( actorStats.GetStatValue( Stat.Cutting ) );
	}
}
