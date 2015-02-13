using UnityEngine;
using System.Collections;

public class FootstepSound : MonoBehaviour 
{
	[SerializeField] AudioSource[] footstepSounds = null;

	[SerializeField] void PlayerWalkSound()
	{
		SoundManager.Play3DSoundAtPosition ( footstepSounds[Random.Range(0, footstepSounds.Length)],
		                                     transform.position - transform.up );
	}
}
