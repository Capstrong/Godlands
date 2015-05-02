using UnityEngine;
using System.Collections;

public class AudioZone : MonoBehaviour 
{
	[SerializeField] AudioClip _musicClip = null;

	void OnTriggerEnter( Collider otherCol )
	{
		if ( otherCol.GetComponentInParent<PlayerActor>() )
		{
			BackgroundMusicManager.PlayMusic( _musicClip );
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon( transform.position, "AudioZone.png", true );
	}
}
