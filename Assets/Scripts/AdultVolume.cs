using UnityEngine;
using System.Collections;

public class AdultVolume : MonoBehaviour
{
	void OnTriggerEnter( Collider other )
	{
		var player = other.GetComponentInParent<PlayerInventory>();

		if ( player && player.isCarryingBuddy )
		{
			BuddyStats buddy = player.backBuddy.hiddenBuddy;

			if ( buddy.isAdult )
			{
				// TODO: Do any vfx for indicating the buddy is an adult.

				AdultManager.SpawnAdult( buddy );
				player.HideBackBuddy();
			}
		}
	}
}
