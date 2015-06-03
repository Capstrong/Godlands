using UnityEngine;
using System.Collections;

public class CheckpointEntranceVolume : MonoBehaviour
{
	CheckpointLifter _lifter;

	void Start()
	{
		_lifter = GetComponentInParent<CheckpointLifter>();
	}

	public void OnTriggerStay( Collider other )
	{
		if ( _lifter.isActive && other.gameObject.GetComponent<PlayerActor>() )
		{
			_lifter.Stay();
		}
	}
}
