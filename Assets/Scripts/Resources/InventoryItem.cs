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

		BuddyItemData buddyItemData =  resourceData as BuddyItemData;

		if ( buddyItemData )
		{
			// All buddies are unique and should have unique data
			// This code looks pretty jank but it pretty much has to be this way
			resourceData = Instantiate<BuddyItemData>( buddyItemData );
			( (BuddyItemData) resourceData ).respawnItem = this;
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
