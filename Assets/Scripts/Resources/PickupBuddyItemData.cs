using UnityEngine;
using System.Collections;

public class PickupBuddyItemData : InventoryItemData
{
	public override bool CanUseItem( PlayerActor player, RaycastHit hitInfo )
	{
		if( needsTarget )
		{
			return !player.inventory.isCarryingBuddy;
		}
		else
		{
			return player.inventory.isCarryingBuddy;
		}
	}

	public override bool UseItem( PlayerActor player, RaycastHit hitInfo )
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
