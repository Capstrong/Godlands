using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerActorStats ) )]
public class SkillFaker : MonoBehaviour
{
	[SerializeField] KeyCode _triggerKey = KeyCode.K;
	[SerializeField] Stat _statToIncrease = Stat.Invalid;

	PlayerActorStats _actorStats = null;

	void Start()
	{
		_actorStats = GetComponent<PlayerActorStats>();
	}
	
	void Update() 
	{
		if ( Input.GetKeyDown( _triggerKey ) )
		{
			_actorStats.IncrementMaxStat( _statToIncrease );
		}
	}
}
