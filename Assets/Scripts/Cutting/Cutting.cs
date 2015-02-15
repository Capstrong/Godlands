using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerActorStats ) )]
public class Cutting : ActorComponent
{
	[SerializeField] LayerMask _cutableLayer = 0;
	[SerializeField] KeyCode _cuttingButton = KeyCode.J;
	[SerializeField] float _cuttingDistance = 0f;
	[SerializeField] GameObject _visualEffect = null;
	[SerializeField] Vector3 _visualOffset = Vector3.zero;

	PlayerActorStats _actorStats = null;

	// Use this for initialization
	void Start ()
	{
		_actorStats = GetComponent<PlayerActorStats>();
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
				GameObject cuttableObj = hitInfo.collider.gameObject;

				Cuttable cuttableComponent = cuttableObj.GetComponent<Cuttable>();

				if ( cuttableComponent )
				{
					Cut( cuttableComponent );
				}

				else
				{
					Debug.LogError( "Attach Cuttable component to " + cuttableObj.name + " at " + cuttableObj.transform.position );
				}
			}
		}
	}

	void Cut( Cuttable cuttableComponent )
	{
		cuttableComponent.Cut( _actorStats.GetStatValue( Stat.Cutting ) );
	}
}
