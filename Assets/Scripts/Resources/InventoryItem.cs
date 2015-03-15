using UnityEngine;
using System.Collections;

public class InventoryItem : MonoBehaviour
{
	public InventoryItemData resourceData;
	[HideInInspector] public GameObject beaconObj = null;
	public bool used = false;

	[Header( "Respawning" )]
	[Tooltip( "Whether this item will repawn after some time" )]
	[SerializeField] bool _canRespawn = true;
	[Tooltip( "In seconds" )]
	[SerializeField] float _respawnTime = 600f;

	Renderer _renderer = null;
	Renderer _beaconRenderer = null;

	void Start()
	{
		_renderer = GetComponentInChildren<Renderer>();

		if ( resourceData as BuddyItemData )
		{
			// All buddies are unique and should have unique data
			resourceData = Instantiate<InventoryItemData>(resourceData);
		}

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
