using UnityEngine;
using System.Collections;

public class AudioZone : MonoBehaviour 
{
	[SerializeField] AudioSource sourceData = null;
	AudioSource playingClip = null;

	void OnTriggerEnter( Collider col )
	{
		if ( col.GetComponent<PlayerActor>() )
		{
			playingClip = SoundManager.instance.Play3DSoundAndFollow( sourceData, transform );
		}
	}

	void OnTriggerExit( Collider col )
	{
		if ( col.GetComponent<PlayerActor>() && playingClip.gameObject )
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
