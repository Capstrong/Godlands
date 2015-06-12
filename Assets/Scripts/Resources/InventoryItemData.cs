using UnityEngine;
using System.Collections;

public class InventoryItemData : ScriptableObject
{
	public Sprite icon;
	[SerializeField] bool needsTarget = true;
	public bool showNumber = true;

	public virtual bool NeedsTarget( PlayerActor player )
	{
		return needsTarget;
	}

	public virtual bool CanUseItem( PlayerActor player, RaycastHit hitInfo = new RaycastHit() )
	{
		return false;
	}

	public virtual bool UseItem( PlayerActor player, RaycastHit hitInfo = new RaycastHit() )
	{
		return false;
	}
}
