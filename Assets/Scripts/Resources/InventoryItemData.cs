using UnityEngine;
using System.Collections;

public class InventoryItemData : ScriptableObject
{
	public Sprite icon;
	public bool needsTarget = true;
	public bool showNumber = true;

	public virtual bool CanUseItem( PlayerActor player, RaycastHit hitInfo )
	{
		return false;
	}

	public virtual bool UseItem( PlayerActor player, RaycastHit hitInfo )
	{
		return false;
	}
}
