using UnityEngine;
using System.Collections;

public class BackBuddy : MonoBehaviour 
{
	Animator animator = null;

	public void PlayEvent( string eventName )
	{
		if( !animator )
		{
			animator = GetComponent<Animator>();
		}

		animator.Play( eventName );
	}
}
