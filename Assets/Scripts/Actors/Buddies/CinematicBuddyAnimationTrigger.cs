using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Animator ) )]
public class CinematicBuddyAnimationTrigger : MonoBehaviour
{
	public string[] _triggerNames = null;

	static int triggerIndex = 0;
	static string[] _shuffledTriggerNames = null;

	void Start () 
	{
		if ( _shuffledTriggerNames == null )
		{
			ShuffleNames();
		}

		string triggerName = _shuffledTriggerNames[ triggerIndex ];

		Animator animator = GetComponent<Animator>();
		animator.Play( triggerName );

		triggerIndex++;

		if ( triggerIndex >= _shuffledTriggerNames.Length )
		{
			triggerIndex = 0;
		}
	}

	void ShuffleNames()
	{
		_shuffledTriggerNames = _triggerNames;
		
		for ( int i = 0; i < _shuffledTriggerNames.Length - 1; i++ )
		{
			int randomIndex = Random.Range( i + 1, _shuffledTriggerNames.Length );
			string temp = _shuffledTriggerNames[ randomIndex ];
			_shuffledTriggerNames[randomIndex] = _shuffledTriggerNames[i];
			_shuffledTriggerNames[i] = temp;
		}
	}
}
