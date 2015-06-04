using UnityEngine;
using System.Collections;

public class InventoryItem : MonoBehaviour
{
	public InventoryItemData resourceData;
	public bool used = false;

	Renderer _renderer = null;
	ResourceHolder _resourceHolder = null;

	public virtual void Start()
	{
		_renderer = GetComponentInChildren<Renderer>();
	}

	public void Initialize( ResourceHolder resourceHolder )
	{
		_resourceHolder = resourceHolder;
	}

	public virtual void Use()
	{
		used = true;
		_renderer.enabled = false;

		if ( _resourceHolder )
		{
			_resourceHolder.Disable();
		}
	}

	public virtual void Enable()
	{
		used = false;
		_renderer.enabled = true;

		if ( _resourceHolder )
		{
			_resourceHolder.Enable();
		}
	}
}
