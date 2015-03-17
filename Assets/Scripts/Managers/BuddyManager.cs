using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuddyManager : SingletonBehaviour<BuddyManager> {

	List<BuddyStats> _buddyStats = new List<BuddyStats>();

	[SerializeField] CheckpointLifter[] checkpointLifters = new CheckpointLifter[1]; // Need these to activate checkpoints
	int checkpointIndex = 0; // Index of next checkpoint to activate

	void Start()
	{
		DayCycleManager.RegisterEndOfDayCallback( DecrementAllBuddyResources );
	}

	public static void RegisterBuddy( BuddyStats buddyStats )
	{
		instance._buddyStats.Add( buddyStats );

		instance.ActivateCheckpoint();
	}

	void ActivateCheckpoint()
	{
		if( checkpointIndex < checkpointLifters.Length && checkpointLifters[checkpointIndex] != null )
		{
			checkpointLifters[checkpointIndex].Activate();
			checkpointIndex++;
		}
	}

	public static void DecrementAllBuddyResources()
	{
		foreach ( BuddyStats buddyStat in instance._buddyStats )
		{
			buddyStat.NightlyEvent();
		}

		instance._buddyStats.RemoveAll( buddyStat => buddyStat.isAlive == false );
	}
}
