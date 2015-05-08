using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BuddyResourceCurve
{
	public const string equationString = "A * X^B + C";

	public float a;
	public float b;
	public float c;

	public float Evaluate( float numBuddies )
	{
		// return 0 for 0 input
		return WadeUtils.IsNotZero( numBuddies ) ? a * Mathf.Pow( numBuddies, b ) + c : 0;
	}
}

public class BuddyManager : SingletonBehaviour<BuddyManager>
{
	// The list doesn't serialize in a SerializableDictionary so it isn't worth it to even try
	Dictionary<Stat,List<BuddyStats>> _buddyStatsDictionary = new Dictionary<Stat,List<BuddyStats>>();

	[SerializeField] float _statPerHappiness = 0f;

	[System.Serializable]
	class StatFloatDict : SerializableDictionary<Stat, float> { }

	[Tooltip( "Minimum stat amount for a stat where any buddies are alive" )]
	[SerializeField] StatFloatDict _minStatDictionary = new StatFloatDict();

	[SerializeField] BuddyResourceCurve _resourceCurve = null;

	[SerializeField] CheckpointLifter[] checkpointLifters = null; // Need these to activate checkpoints
	[ReadOnly] int checkpointIndex = 0; // Index of next checkpoint to activate

	[ReadOnly]
	[SerializeField] PlayerStats _playerStats = null;

	void Start()
	{
		DayCycleManager.RegisterEndOfDayCallback( NightlyEvent );
		_buddyStatsDictionary.Add( Stat.Cutting, new List<BuddyStats>() );
		_buddyStatsDictionary.Add( Stat.Gliding, new List<BuddyStats>() );
		_buddyStatsDictionary.Add( Stat.Stamina, new List<BuddyStats>() );
		_playerStats = FindObjectOfType<PlayerStats>();
	}

	public static void RegisterBuddy( BuddyStats buddyStats )
	{
		instance._buddyStatsDictionary[buddyStats.itemData.stat].Add( buddyStats );

		instance.ActivateCheckpoint();
	}

	void ActivateCheckpoint()
	{
		if( checkpointIndex < checkpointLifters.Length && checkpointLifters[checkpointIndex] )
		{
			checkpointLifters[checkpointIndex].Activate();
			checkpointIndex++;
		}
	}

	private void NightlyEvent()
	{
		foreach ( Stat statKey in _buddyStatsDictionary.Keys )
		{
			int numBuddiesOfType = _buddyStatsDictionary[statKey].Count;

			if ( numBuddiesOfType != 0 )
			{
				float totalResourceDrain = _resourceCurve.Evaluate( numBuddiesOfType );
				int drainPerBuddy = (int) Mathf.Round( totalResourceDrain / numBuddiesOfType );

				foreach ( BuddyStats buddyStat in _buddyStatsDictionary[statKey] )
				{
					if ( buddyStat.isAlive )
					{
						buddyStat.DecrementResources( drainPerBuddy );
						buddyStat.AffectHappinessWithHunger();
						buddyStat.AgeUp();
					}
				}

				_buddyStatsDictionary[statKey].RemoveAll( buddyStat => buddyStat.isAlive == false );

				RecalculateStat( statKey );
			}
		}
	}

	public static void RecalculateStat( Stat stat )
	{
		if ( instance._buddyStatsDictionary[stat].Count == 0 )
		{
			instance._playerStats.SetMaxStat( stat, 0f );
			return;
		}

		float aggregateHappiness = 0f;

		foreach ( BuddyStats buddyStat in instance._buddyStatsDictionary[stat] )
		{
			aggregateHappiness += buddyStat.happiness;
		}

		instance._playerStats.SetMaxStat( stat, Mathf.Max( aggregateHappiness * instance._statPerHappiness, instance._minStatDictionary[stat] ) );
	}
}
