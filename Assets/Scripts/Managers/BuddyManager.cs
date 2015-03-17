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
	public List<BuddyStats> _buddyStats = new List<BuddyStats>();

	// total resources needed per buddy type = a * ( # buddies of that type) ^ b + c
	[SerializeField] BuddyResourceCurve _resourceCurve;

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
			buddyStat.NightlyEvent();
		}

		instance._buddyStats.RemoveAll( buddyStat => buddyStat.isAlive == false );
	}
}
