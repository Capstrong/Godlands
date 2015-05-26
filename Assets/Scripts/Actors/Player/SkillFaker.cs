﻿using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerStats ) )]
public class SkillFaker : MonoBehaviour
{
	[SerializeField] KeyCode _triggerKey = KeyCode.K;
	[SerializeField] Stat _statToIncrease = Stat.Invalid;
	[SerializeField] float _changeAmount = 0.1f;

	PlayerStats _actorStats = null;

	void Start()
	{
		_actorStats = GetComponent<PlayerStats>();
	}
	
	void Update() 
	{
		if ( Input.GetKeyDown( _triggerKey ) )
		{
			_actorStats.SetMaxStat( _statToIncrease, _actorStats.GetStatMaxValue( _statToIncrease ) + _changeAmount );
		}
	}
}