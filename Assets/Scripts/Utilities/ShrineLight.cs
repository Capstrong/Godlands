using UnityEngine;
using System.Collections;

public class ShrineLight : MonoBehaviour 
{
	Renderer _renderer = null;
	PlayerInventory _playerInventory = null;

	void Awake ()
	{
		_renderer = GetComponent<Renderer>();
	}

	void Update()
	{
		if( !_playerInventory )
		{
			_playerInventory = FindObjectOfType<PlayerInventory>();

			if( _playerInventory )
			{
				_playerInventory.BuddyPickupCallback += EnableLight;
				_playerInventory.BuddyPutDownCallback += DisableLight;
			}
		}
	}

	void EnableLight( BuddyStats buddyStats )
	{
		if( buddyStats.isOfAge )
		{
			_renderer.material.SetColor( "_EmissionColor", Color.white );
		}
	}

	void DisableLight( BuddyStats buddyStats )
	{
		_renderer.material.SetColor( "_EmissionColor", Color.black );
	}
}
