using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuddyManager : SingletonBehaviour<BuddyManager> {

	List<BuddyStats> _buddyStats = new List<BuddyStats>();

	void Start()
	{
		DayCycleManager.RegisterEndOfDayCallback( DecrementAllBuddyResources );
	}

	public static void RegisterBuddy( BuddyStats buddyStats )
	{
		instance._buddyStats.Add( buddyStats );
	}

	public static void DecrementAllBuddyResources()
	{
		foreach ( BuddyStats buddyStat in instance._buddyStats )
		{
			buddyStat.DecrementResources();
		}

		instance._buddyStats.RemoveAll( buddyStat => buddyStat.isAlive == false ); // Predicate statement
	}
}
