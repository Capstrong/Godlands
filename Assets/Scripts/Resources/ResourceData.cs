using UnityEngine;
using System.Collections;

public class ResourceData : InventoryItemData
{
	public override bool CanUseItem( PlayerActor player, RaycastHit hitInfo = new RaycastHit() )
	{
		if( hitInfo.transform )
		{
			BuddyStats buddy = hitInfo.transform.GetComponentInChildren<BuddyStats>();
			return buddy && buddy.IsHungry();
		}
		else
		{
			return false;
		}
	}

	public override bool UseItem( PlayerActor player, RaycastHit hitInfo = new RaycastHit() )
	{
		return player.inventory.CheckGiveResources( hitInfo );
	}
}
