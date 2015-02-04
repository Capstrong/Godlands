using UnityEngine;
using System.Collections;

public class AudioZone : MonoBehaviour 
{
	[SerializeField] AudioSource sourceData = null;
	AudioSource playingClip = null;

	void OnTriggerEnter( Collider otherCol )
	{
		if ( otherCol.GetComponent<PlayerActor>() )
		{
			playingClip = SoundManager.instance.Play3DSoundAndFollow( sourceData, transform );
		}
	}

	void OnTriggerExit( Collider otherCol )
	{
		if ( otherCol.GetComponent<PlayerActor>() && playingClip )
		{
			playingClip.Stop();
			playingClip = null;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon( transform.position, "AudioZone.png", true );
	}
}
