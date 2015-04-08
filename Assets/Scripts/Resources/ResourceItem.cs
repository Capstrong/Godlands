using UnityEngine;
using System.Collections;

public class ResourceItem : InventoryItem
{
	[Tooltip( "In seconds" )]
	[SerializeField] float _respawnTime = 600f;

	public override void Use()
	{
		base.Use();

		Invoke( "Enable", _respawnTime );
	}
}
