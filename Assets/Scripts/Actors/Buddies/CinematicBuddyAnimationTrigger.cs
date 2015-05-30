using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Animator ) )]
public class CinematicBuddyAnimationTrigger : MonoBehaviour
{
	public string[] _triggerNames = null;

	static int triggerIndex = 0;

	void Start () 
	{
		string triggerName = _triggerNames[ triggerIndex ];

		Animator animator = GetComponent<Animator>();
		animator.Play( triggerName );

		// Make it kind of random while also encouraging it to move through the list and not hit the same thing twice
		triggerIndex += Random.Range( 1, 3 );

		if ( triggerIndex >= _triggerNames.Length )
		{
			triggerIndex = 0;
		}
	}
}
