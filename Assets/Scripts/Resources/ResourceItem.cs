using UnityEngine;
using System.Collections;

public class ResourceItem : InventoryPickupItem
{
	ResourceSpawner _resourceSpawner = null;

	public void Initialize( ResourceSpawner spawner )
	{
		_resourceSpawner = spawner;
	}

	public override void Use()
	{
		base.Use();
	
		_resourceSpawner.MarkForRespawn( this );
	}
}
