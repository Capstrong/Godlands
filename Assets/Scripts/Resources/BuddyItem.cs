using UnityEngine;
using System.Collections;

public class BuddyItem : InventoryItem
{
	[SerializeField] GameObject _internals = null;

	public override void Start()
	{
		base.Start();

		BuddyItemData buddyData = Instantiate<BuddyItemData>( (BuddyItemData) resourceData );
		buddyData.respawnItem = this;

		resourceData = buddyData;
	}

	public override void Use()
	{
		base.Use();

		_internals.SetActive( false );
	}
}
