using UnityEngine;
using System.Collections;

public class AdultVolume : MonoBehaviour
{
	void OnTriggerEnter( Collider other )
	{
		Debug.Log("Adult trigger enter.");

		var player = other.GetComponentInParent<PlayerInventory>();

		if ( player && player.isCarryingBuddy )
		{
			BuddyStats buddy = player.backBuddy._hiddenBuddy;

			if ( buddy.isAdult )
			{
				// Make the buddy into an adult.

				// TODO: Do any vfx for indicating the buddy is an adult.

				Debug.Log("Made an adult");

				AdultManager.SpawnAdult( buddy );
				player.HideBackBuddy();
			}
		}
	}
}
