using UnityEngine;
using System.Collections;

public class FootstepSound : MonoBehaviour {

	[SerializeField] void PlayerWalkSound(){
		SoundManager.instance.PlaySoundAtPosition ("Footstep", transform.position-transform.up);
	}
}
