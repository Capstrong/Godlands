using UnityEngine;
using System.Collections;

public class InventoryItem : MonoBehaviour
{
	public InventoryItemData resourceData;
	public int amount = 1;
	[Tooltip("In seconds")]
	[SerializeField] float _respawnTime = 600f;

	public bool used = false;

	Renderer _renderer = null;
	public GameObject beaconObj;
	Renderer _beaconRenderer = null;

	void Start()
	{
		_renderer = GetComponent<Renderer>();
		_beaconRenderer = beaconObj.GetComponent<Renderer>();
	}

	public void Use()
	{
		used = true;
		_renderer.enabled = false;
		Invoke( "Enable", _respawnTime );
		_beaconRenderer.enabled = false;
	}

	public void Enable()
	{
		used = false;
		_renderer.enabled = true;
		_beaconRenderer.enabled = true;
	}
}
