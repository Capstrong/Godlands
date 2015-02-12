using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerActorStats ) )]
public class CuttingA : ActorComponent
{

	[SerializeField] LayerMask _cutableLayer = 0;
	[SerializeField] KeyCode _cuttingButton;
	[SerializeField] float _cuttingDistance = 0f;
	[SerializeField] GameObject _visualEffect = null;
	[SerializeField] Vector3 _visualOffset;

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
			Vector3 rotatedOffset = actor.model.transform.rotation * _visualOffset;

			Instantiate( _visualEffect, transform.position + rotatedOffset, actor.model.transform.rotation );

			RaycastHit hitInfo;
			Physics.Raycast( new Ray( transform.position, actor.model.transform.forward ), out hitInfo, _cuttingDistance, _cutableLayer );

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
					Debug.LogError( "Attach Cutable component to " + cutableObj.name + " at " + cutableObj.transform.position );
				}
			}
		}
	}

	void Cut( CutableA cutableComponent )
	{
		cutableComponent.Cut( actorStats.GetStatValue( Stat.Cutting ) );
	}
}
