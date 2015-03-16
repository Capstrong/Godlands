using UnityEngine;
using System.Collections;

public class BuddyItemData : InventoryItemData
{
	public GameObject buddyPrefab;
	public Stat stat;
	public Color statColor;
	[ReadOnly( "Respawn Item" ), Tooltip( "This is so that the item can be re-enabled if the buddy dies" )]
	public InventoryItem respawnItem;
}
