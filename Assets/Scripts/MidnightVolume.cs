using UnityEngine;
using System.Collections;

public class MidnightVolume : MonoBehaviour
{
	[SerializeField] float _midnightFadeTime = 2.0f;

	void OnTriggerEnter( Collider other )
	{
		if ( other.GetComponent<PlayerActor>() )
		{
			DayCycleManager.TriggerMidnight( _midnightFadeTime );
		}
	}
}
