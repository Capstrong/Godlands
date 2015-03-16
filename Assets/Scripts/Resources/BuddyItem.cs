using UnityEngine;
using System.Collections;

public class BuddyItem : InventoryItem
{
	public override void Start()
	{
		base.Start();

		BuddyItemData buddyData = Instantiate<BuddyItemData>( (BuddyItemData) resourceData );
		buddyData.respawnItem = this;

		resourceData = buddyData;
	}
}
