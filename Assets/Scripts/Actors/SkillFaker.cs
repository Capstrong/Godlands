using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerActorStats ) )]
public class SkillFaker : MonoBehaviour
{
	[SerializeField] KeyCode _triggerKey;
	[SerializeField] Stat _statToIncrease;

	PlayerActorStats _actorStats = null;

	void Start()
	{
		_actorStats = GetComponent<PlayerActorStats>();
	}
	
	void Update() 
	{
		if ( Input.GetKeyDown( _triggerKey ) )
		{
			_actorStats.IncrementMaxStat( Stat.Cutting );
		}
	}
}
