using UnityEngine;
using System.Collections;

public class AudioZone : MonoBehaviour 
{
	[SerializeField] AudioSource sourceData = null;
	AudioSource playingClip = null;

	void OnTriggerEnter( Collider col )
	{
		Debug.Log( "Do the thing" );

		PlayerActor pa = col.GetComponent<PlayerActor>();
		if ( pa )
		{

			playingClip = SoundManager.instance.Play3DSoundAndFollow( sourceData, transform );
		}
	}

	void OnTriggerExit( Collider col )
	{
		PlayerActor pa = col.GetComponent<PlayerActor>();
		if ( pa && playingClip )
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
