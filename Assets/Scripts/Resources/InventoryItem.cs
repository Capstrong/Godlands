using UnityEngine;
using System.Collections;

public class InventoryItem : MonoBehaviour
{
	public InventoryItemData resourceData;
	[HideInInspector] public GameObject beaconObj = null;
	public bool used = false;

	protected Renderer _renderer = null;
	protected Renderer _beaconRenderer = null;

	public virtual void Start()
	{
		_renderer = GetComponentInChildren<Renderer>();

		if ( beaconObj )
		{
			_beaconRenderer = beaconObj.GetComponent<Renderer>();
		}
	}

	public virtual void Use()
	{
		used = true;
		_renderer.enabled = false;

		if ( _beaconRenderer )
		{
			_beaconRenderer.enabled = false;
		}
	}

	public void Enable()
	{
		used = false;
		_renderer.enabled = true;
		if ( _beaconRenderer )
		{
			_beaconRenderer.enabled = true;
		}
	}
}
