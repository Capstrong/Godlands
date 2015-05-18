using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Stat
{
	Invalid = -1,
	Stamina,
	Gliding,
	Cutting,
}

[System.Serializable]
public class StatObject
{
	public float startingMax = 0.0f;
	public float useRate = 0.0f; // Stat units per second
	public float rechargeRate = 0.0f;
	public float currentMax = 0.0f;
	public float currentValue = 0.0f;
	public bool  isUsing = false;
	public float rechargeDelayTime = 0.0f; // Seconds

	[ReadOnly]
	public float rechargeTimer = 0.0f;
}

[System.Serializable]
public class StatDictionary : SerializableDictionary<Stat, StatObject> { } // Necessary for serialization

[RequireComponent( typeof(PlayerControls) )]
public class PlayerStats : ActorComponent
{
	[SerializeField] StatDictionary _statDictionary = new StatDictionary();

	public override void Awake()
	{
		base.Awake();

		foreach ( KeyValuePair<Stat, StatObject> pair in _statDictionary )
		{
			pair.Value.currentMax = pair.Value.startingMax;
			pair.Value.currentValue = pair.Value.startingMax;
		}
	}

	public void SetMaxStat( Stat stat, float maxValue )
	{
		DebugUtils.Assert( maxValue >= 0.0f, "Max stat value must be greater than or equal to 0." );

		StatObject statObject = _statDictionary[stat];
		statObject.currentMax = maxValue;

		SkillUIManager.UpdateMaxStatUI( stat, maxValue );
	}

	public bool CanUseStat( Stat stat )
	{
		return ( _statDictionary[stat].currentValue > 0.0f );
	}

	public void StartUsingStat( Stat stat )
	{
		_statDictionary[stat].isUsing = true;
	}

	public void StopUsingStat( Stat stat )
	{
		_statDictionary[stat].isUsing = false;
	}

	public float GetStatValue( Stat stat )
	{
		return _statDictionary[stat].currentValue;
	}

	public float GetStatMaxValue( Stat stat )
	{
		return _statDictionary[stat].currentMax;
	}

	void Update()
	{
		foreach ( KeyValuePair<Stat, StatObject> pair in _statDictionary )
		{
			StatObject statObject = pair.Value;

			if ( statObject.isUsing )
			{
				statObject.rechargeTimer = 0.0f;

				statObject.currentValue -= statObject.useRate * Time.deltaTime;

				if ( statObject.currentValue <= 0 )
				{
					statObject.currentValue = 0;

					StopUsingStat( pair.Key );
				}

				SkillUIManager.UpdateStatUI( pair.Key, pair.Value.currentValue );
			}
			else
			{
				statObject.rechargeTimer += Time.deltaTime;

				if ( statObject.rechargeTimer > statObject.rechargeDelayTime )
				{
					statObject.currentValue += statObject.rechargeRate * Time.deltaTime;

					if ( statObject.currentValue > statObject.currentMax )
					{
						statObject.currentValue = statObject.currentMax;
					}

					SkillUIManager.UpdateStatUI( pair.Key, pair.Value.currentValue );
				}
			}
		}
	}
}
