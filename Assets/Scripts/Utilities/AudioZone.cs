using UnityEngine;
using System.Collections;

public class AudioZone : MonoBehaviour 
{
	[SerializeField] AudioSource _musicSource = null;

	void OnTriggerEnter( Collider otherCol )
	{
		if ( otherCol.GetComponentInParent<PlayerActor>() )
		{
			BackgroundMusicManager.PlayMusic( _musicSource );
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon( transform.position, "AudioZone.png", true );
	}
}
