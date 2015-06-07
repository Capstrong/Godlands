using UnityEngine;
using System.Collections;

public class ResourceItem : InventoryPickupItem
{
	[Tooltip( "In seconds" )]
	[SerializeField] float _respawnTime = 600f;

	ResourceSpawner _resourceSpawner = null;

	public void Awake()
	{
		// Resource spawner is a parent of the resource holder.
		_resourceSpawner = GetComponent<Transform>().parent.gameObject.GetComponentInParent<ResourceSpawner>();
	}

	public override void Use()
	{
		base.Use();
	
		// Do stuff to notify resource spawner.
	}
}
