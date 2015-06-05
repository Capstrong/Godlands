using UnityEngine;
using System.Collections;

public class PickupBuddyItemData : InventoryItemData
{
	public override bool CanUseItem( PlayerActor player, RaycastHit hitInfo = new RaycastHit() )
	{
		if( player.inventory.isCarryingBuddy )
		{	
			// TODO: Feedback and effect to explain why the buddy can't be spawned outside the garden
			return MathUtils.IsWithinInfiniteVerticalCylinders( player.inventory.GetBuddySpawnPosition(), LimitsManager.colliders );
		}
		else
		{
			return hitInfo.transform && hitInfo.transform.GetComponentInChildren<BuddyStats>();
		}
	}

	public override bool UseItem( PlayerActor player, RaycastHit hitInfo = new RaycastHit() )
	{
		if( player.inventory.isCarryingBuddy )
		{
			needsTarget = true;
			return player.inventory.CheckPutDownBuddy();
		}
		else
		{
			needsTarget = false;
			return player.inventory.CheckPickUpBuddy( hitInfo );
		}
	}
}
