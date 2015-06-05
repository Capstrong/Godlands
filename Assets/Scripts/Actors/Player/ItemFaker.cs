using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerInventory ) )]
public class ItemFaker : MonoBehaviour
{
	[SerializeField] KeyCode _triggerKey = KeyCode.K;
	[SerializeField] InventoryItemData _itemData = null;

	PlayerInventory _inventory = null;

	void Start()
	{
		_inventory = GetComponent<PlayerInventory>();
	}
	
	void Update()
	{
		if ( Input.GetKey( KeyCode.LeftAlt ) && Input.GetKeyDown( _triggerKey ) )
		{
			_inventory.PickupItem( _itemData );
		}
	}
}
