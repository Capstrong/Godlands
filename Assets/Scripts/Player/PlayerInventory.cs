﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof( PlayerCamera ), typeof( PlayerStats ) )]
public class PlayerInventory : ActorComponent
{
	InventoryScrollBar _inventoryBar;

	[SerializeField] GameObject _resourcePopPrefab = null;

	[SerializeField] List<BuddyTag> _buddies = new List<BuddyTag>();
	public List<BuddyTag> buddies
	{
		get { return _buddies; }
	}

	[SerializeField] float _lookOverrideDuration = 0.5f;

	// types and current count
	[System.Serializable]
	public class InventoryDictionary : SerializableDictionary<InventoryItemData, int> { }

	[ReadOnly("Inventory")]
	[SerializeField] InventoryDictionary _inventory = new InventoryDictionary();

	// currently held types to show on UI bar
	List<InventoryItemData> _heldResources = new List<InventoryItemData>();

	int _resourceIndex = 0; // Currently selected resource on UI bar
	GameObject _heldResource = null; 

	PlayerActor _playerActor = null;

	[SerializeField] BackBuddy _backBuddy = null;
	GameObject _hiddenBuddy = null;
	bool _isCarryingBuddy = false;


	[SerializeField] LayerMask _buddyLayer = 0;
	public LayerMask buddyLayer
	{
		get { return _buddyLayer; }
	}

	[SerializeField] float _buddySpawnDistance = 3.0f;
	
	AxisButtons _altScrollButton = new AxisButtons("Alt_Scroll");

	public override void Awake()
	{
		base.Awake();

		_playerActor = GetComponent<PlayerActor>();
	}

	// Use this for initialization
	void Start()
	{
		_inventoryBar = GameObject.FindObjectOfType<InventoryScrollBar>();
		DebugUtils.Assert( _inventoryBar, "There must be an InventoryScrollBar object in the scene." );

		DayCycleManager.RegisterEndOfDayCallback( HideBackBuddy );

		if ( _heldResources.Count > 0 )
		{
			SpawnResourceObject();
		}
	}

	// Update is called once per frame
	void Update()
	{
		_altScrollButton.Update();
		CheckScroll();
	}

	void CheckScroll()
	{
		if ( _heldResources.Count <= 1 )
		{
			// Nothing to scroll
			return;
		}

		float scrollAmount = InputUtils.GetAxis("Scroll");

		if ( WadeUtils.IsZero( scrollAmount ) )
		{
			if ( _altScrollButton.positiveDown )
			{
				scrollAmount = 1f;
			}
			else if ( _altScrollButton.negativeDown )
			{
				scrollAmount = -1f;
			}
		}

		if ( !WadeUtils.IsZero( scrollAmount ) )
		{
			int nextIndex = _resourceIndex + ( scrollAmount < 0f ? -1 : 1 );
			_resourceIndex = MathUtils.Mod( nextIndex, _heldResources.Count );
			SpawnResourceObject();
		}
	}

	public bool CanUseItemWithoutTarget()
	{
		return _heldResources.Count > 0 &&
		       !_heldResources[_resourceIndex].needsTarget;
	}

	public void UseItem()
	{
		DebugUtils.Assert( _heldResources[_resourceIndex] is BuddyItemData, "Cannot use item without target." );

		SpawnBuddy();
	}

	public bool UseItemWithTarget( RaycastHit hitInfo )
	{
		if ( _heldResources.Count > 0 )
		{
			if ( _heldResources[_resourceIndex] is ResourceData )
			{
				return CheckGiveResources( hitInfo );
			}
			else
			{
				return SpawnBuddy();
			}
		}
		else
		{
			return false;
		}
	}

	bool CheckGiveResources( RaycastHit hitInfo )
	{
		DebugUtils.Assert( hitInfo.transform != null, "hitInfo must have data." );

		BuddyStats buddyStats = hitInfo.transform.GetComponent<BuddyStats>();

		GodTag godTag = GetComponent<GodTag>(); // For checking if this actor owns the buddy

		if ( buddyStats &&
			 buddyStats.isAlive &&
			 ( buddyStats.owner == null || buddyStats.owner == godTag ) )
		{
			buddyStats.owner = godTag;
			GiveResource( buddyStats );

			if( !_isCarryingBuddy )
			{
				PickUpBuddy( hitInfo, buddyStats );
			}

			// look at the buddy
			_playerActor.physics.OverrideLook(
				buddyStats.GetComponent<Transform>().position - GetComponent<Transform>().position,
				_lookOverrideDuration );
			return true;
		}
		else
		{
			return false;
		}
	}

	void PickUpBuddy( RaycastHit hitInfo, BuddyStats buddyStats )
	{
		BuddyShaper buddyShaper = hitInfo.transform.GetComponentInChildren<BuddyShaper>();
		if( buddyShaper )
		{
			_backBuddy.gameObject.SetActive( true ); // Buddy is always on back, we just hide it
			_backBuddy.CopyBuddy( buddyShaper.skinnedMeshRend ); // Copy buddy style to backBuddy prototype
			
			_hiddenBuddy = buddyStats.gameObject;
			_hiddenBuddy.SetActive( false );
			
			DayCycleManager.RegisterEndOfDayCallback( ReenableHiddenBuddy );
			
			_isCarryingBuddy = true;
		}
	}

	void GiveResource( BuddyStats buddyStats )
	{
		buddyStats.GiveResource( (actor as PlayerActor).stats, (ResourceData)_heldResources[_resourceIndex] );
		_inventory[_heldResources[_resourceIndex]]--;

		UpdateResourceList();
	}

	bool SpawnBuddy()
	{
		if ( !MathUtils.IsWithinInfiniteVerticalCylinders( transform.position + transform.forward * _buddySpawnDistance, LimitsManager.colliders ) )
		{
			// TODO: Feedback and effect to explain why the buddy can't be spawned outside the garden
			return false;
		}

		Vector3 spawnLocation;

		RaycastHit hitInfo;

		Physics.Raycast( new Ray( transform.position, transform.forward), out hitInfo, _buddySpawnDistance );

		if ( hitInfo.transform )
		{
			spawnLocation = hitInfo.point;
		}
		else
		{
			spawnLocation = transform.position + transform.forward * _buddySpawnDistance;
		}

		BuddyItemData buddyItemData = (BuddyItemData)_heldResources[_resourceIndex];
		BuddyStats newBuddy = ( Instantiate( buddyItemData.buddyPrefab,
		                                     spawnLocation,
		                                     Quaternion.identity ) as GameObject ).GetComponent<BuddyStats>();
		newBuddy.Initialize( GetComponent<GodTag>(), buddyItemData );

		_buddies.Add( newBuddy.GetComponent<BuddyTag>() );

		_inventory[_heldResources[_resourceIndex]]--;
		UpdateResourceList();
		newBuddy.RecalculateStat();
		return true;
	}

	public void ReenableHiddenBuddy()
	{
		_hiddenBuddy.SetActive( true );
	}

	public void HideBackBuddy()
	{
		_backBuddy.gameObject.SetActive( false );
		_isCarryingBuddy = false;
	}

	void PickupItem( InventoryItemData itemData )
	{
		if ( !_inventory.ContainsKey( itemData ) )
		{
			_inventory[itemData] = 0;
		}

		_inventory[itemData]++;
		UpdateResourceList();
	}

	void UpdateResourceList()
	{
		_heldResources.Clear();

		foreach ( InventoryItemData itemData in _inventory.Keys )
		{
			if ( _inventory[itemData] > 0 )
			{
				_heldResources.Add( itemData );
			}
		}

		_resourceIndex = ( _heldResources.Count > 0 ? _resourceIndex % _heldResources.Count : 0 );

		if ( _heldResources.Count == 0 )
		{
			if ( _heldResource )
			{
				Destroy( _heldResource );
			}

			_inventoryBar.NullInventoryBar();
		}
		else if ( _heldResources.Count > 0 )
		{
			SpawnResourceObject();
			_inventoryBar.UpdateInventoryBar( _resourceIndex, _heldResources.ToArray() );
		}
	}

	void SpawnResourceObject()
	{
		_inventoryBar.UpdateInventoryBar( _resourceIndex, _heldResources.ToArray() );

		if ( _heldResource )
		{
			Destroy( _heldResource );
		}

		if ( _heldResources[_resourceIndex].prefab )
		{
			_heldResource = WadeUtils.Instantiate( _heldResources[_resourceIndex].prefab );
			_heldResource.transform.parent = actor.GetBoneAtLocation( BoneLocation.RHand );
			WadeUtils.ResetTransform( _heldResource.transform, true );
		}
	}

	void OnTriggerEnter( Collider other )
	{
		InventoryItem inventoryItem = other.gameObject.GetComponentInChildren<InventoryItem>();
		if ( inventoryItem && !inventoryItem.used )
		{
			inventoryItem.Use();

			PickupItem( inventoryItem.resourceData );

			WadeUtils.TempInstantiate( _resourcePopPrefab, other.transform.position, Quaternion.identity, 1f );
		}
	}
}
