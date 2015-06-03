using UnityEngine;
using System.Collections;

public class CheckpointExitVolume : MonoBehaviour
{
	CheckpointLifter _lifter;

	void Start()
	{
		_lifter = GetComponentInParent<CheckpointLifter>();
	}

	public void OnTriggerExit( Collider other )
	{
		if ( _lifter.isActive && other.gameObject.GetComponent<PlayerActor>() )
		{
			_lifter.Exit();
		}
	}
}