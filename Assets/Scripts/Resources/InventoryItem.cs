using UnityEngine;
using System.Collections;

public class InventoryItem : MonoBehaviour
{
	public InventoryItemData resourceData;
	public int amount = 1;
	[Tooltip( "In seconds" )]
	[SerializeField] float _respawnTime = 600f;
	[Tooltip( "Whether this item will repawn after some time" )]
	[SerializeField] bool _canRespawn = true;

	public bool used = false;

	Renderer _renderer = null;
	public GameObject beaconObj = null;
	Renderer _beaconRenderer = null;

	void Start()
	{
		_renderer = GetComponentInChildren<Renderer>();
		if ( beaconObj )
		{
			_beaconRenderer = beaconObj.GetComponent<Renderer>();
		}
	}

	public void Use()
	{
		used = true;
		_renderer.enabled = false;

		if ( _canRespawn )
		{
			Invoke( "Enable", _respawnTime );
		}

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
