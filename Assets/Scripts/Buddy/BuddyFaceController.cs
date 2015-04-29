using UnityEngine;
using System.Collections;

public class BuddyFaceController : MonoBehaviour 
{
	Animator animator = null;

	public void PlayEvent( string eventName )
	{
		if( !animator )
		{
			animator = GetComponent<Animator>();
		}
		
		animator.Play( eventName, 1 );
	}
}
