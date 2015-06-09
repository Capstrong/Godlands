using UnityEngine;
using System.Collections;

public class BuddyItemData : InventoryItemData
{
	public GameObject buddyPrefab;
	public Stat stat;
	public Color statColor;
	[ReadOnly( "Respawn Item" ), Tooltip( "This is so that the item can be re-enabled if the buddy dies" )]
	public InventoryPickupItem respawnItem;

	public override bool CanUseItem( PlayerActor player, RaycastHit hitInfo = new RaycastHit() )
	{
		// TODO: Feedback and effect to explain why the buddy can't be spawned outside the garden
		return MathUtils.IsWithinInfiniteVerticalCylinders( player.inventory.GetBuddySpawnPosition(), LimitsManager.colliders ) ;
	}

	public override bool UseItem( PlayerActor player, RaycastHit hitInfo = new RaycastHit() )
	{
		if ( hitInfo.collider )
		{
			return false;
		}
		else
		{
			return player.inventory.SpawnBuddy();
		}
	}
}
