using UnityEngine;
using System.Collections;

public class CheckpointVolume : MonoBehaviour {

	CheckpointLifter _lifter;

	[Tooltip( "In seconds" )]
	[SerializeField] float _activationDelay = 0f;

	// Use this for initialization
	void Start ()
	{
		_lifter = GetComponentInParent<CheckpointLifter>();
	}
	
	public void OnTriggerEnter( Collider other )
	{
		if ( !_lifter.isActive && other.gameObject.GetComponent<PlayerActor>() )
		{
			Invoke( "Activate", _activationDelay );
		}
	}

	void Activate()
	{
		_lifter.Activate();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Transform transform = GetComponent<Transform>();
		Gizmos.matrix = transform.localToWorldMatrix;

		BoxCollider collider = GetComponent<BoxCollider>();
		Gizmos.DrawWireCube( collider.center, collider.size );
	}

}
