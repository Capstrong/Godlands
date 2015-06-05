using UnityEngine;
using System.Collections;

public class ResourceData : InventoryItemData
{
	public override bool CanUseItem( PlayerActor player, RaycastHit hitInfo )
	{
		BuddyStats buddy = hitInfo.transform.GetComponentInChildren<BuddyStats>();
		return buddy && buddy.IsHungry();
	}

	public override bool UseItem( PlayerActor player, RaycastHit hitInfo )
	{
		return player.inventory.CheckGiveResources( hitInfo );
	}
}
