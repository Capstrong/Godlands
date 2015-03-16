﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof( PlayerCamera ), typeof( PlayerStats ) )]
public class PlayerInventory : ActorComponent
{
	InventoryScrollBar inventoryBar;

	[SerializeField] GameObject resourcePopPrefab = null;

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
	[SerializeField] InventoryDictionary inventory = new InventoryDictionary();

	// currently held types to show on UI bar
	List<InventoryItemData> heldResources = new List<InventoryItemData>();

	int resourceIndex = 0;
	GameObject heldResource;

	PlayerActor _playerActor;

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
		inventoryBar = GameObject.FindObjectOfType<InventoryScrollBar>();
		DebugUtils.Assert( inventoryBar, "There must be an InventoryScrollBar object in the scene." );

		if ( heldResources.Count > 0 )
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
		if ( heldResources.Count <= 1 )
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
			int nextIndex = resourceIndex + ( scrollAmount < 0f ? -1 : 1 );
			resourceIndex = MathUtils.Mod( nextIndex, heldResources.Count );
			SpawnResourceObject();
		}
	}

	public bool CanUseItemWithoutTarget()
	{
		return heldResources.Count > 0 &&
		       !heldResources[resourceIndex].needsTarget;
	}

	public void UseItem()
	{
		DebugUtils.Assert( heldResources[resourceIndex] is BuddyItemData, "Cannot use item without target." );

		SpawnBuddy();
	}

	public void UseItemWithTarget( RaycastHit hitInfo )
	{
		if ( heldResources.Count > 0 )
		{
			if ( heldResources[resourceIndex] is ResourceData )
			{
				CheckGiveResources( hitInfo );
			}
			else
			{
				SpawnBuddy();
			}
		}
	}

	void CheckGiveResources( RaycastHit hitInfo )
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

			// look at the buddy
			_playerActor.physics.OverrideLook(
				buddyStats.GetComponent<Transform>().position - GetComponent<Transform>().position,
				_lookOverrideDuration );
		}
	}

	void GiveResource( BuddyStats buddyStats )
	{
		buddyStats.GiveResource( (actor as PlayerActor).stats, (ResourceData)heldResources[resourceIndex] );
		inventory[heldResources[resourceIndex]]--;

		UpdateResourceList();
	}

	void SpawnBuddy()
	{
		if ( !MathUtils.IsWithinInfiniteVerticalCylinders( transform.position + transform.forward * _buddySpawnDistance, LimitsManager.colliders ) )
		{
			// TODO: Feedback and effect to explain why the buddy can't be spawned outside the garden
			return;
		}

		BuddyItemData buddyItemData = (BuddyItemData)heldResources[resourceIndex];
		BuddyStats newBuddy = ( Instantiate( buddyItemData.buddyPrefab,
		                                     transform.position + transform.forward * _buddySpawnDistance,
		                                     Quaternion.identity ) as GameObject ).GetComponent<BuddyStats>();
		newBuddy.owner = GetComponent<GodTag>();
		newBuddy.statType = buddyItemData.stat; // This also initializes the stat on the player
		newBuddy.itemData = buddyItemData;

		// this could be bad, should probably run it by Chris

		// will need to write a shader with color mask for the buddies so we can change just the onesie color
		// once that's in this will be changed to material.SetColor("_ColorPropertyName", buddyItemData.statColor) - Chris
		newBuddy.bodyRenderer.material.color = buddyItemData.statColor;

		_buddies.Add( newBuddy.GetComponent<BuddyTag>() );

		inventory[heldResources[resourceIndex]]--;
		UpdateResourceList();
	}

	void PickupItem( InventoryItemData itemData )
	{
		if ( !inventory.ContainsKey( itemData ) )
		{
			inventory[itemData] = 0;
		}

		inventory[itemData]++;
		UpdateResourceList();
	}

	void UpdateResourceList()
	{
		heldResources.Clear();

		foreach ( InventoryItemData itemData in inventory.Keys )
		{
			if ( inventory[itemData] > 0 )
			{
				heldResources.Add( itemData );
			}
		}

		resourceIndex = ( heldResources.Count > 0 ? resourceIndex % heldResources.Count : 0 );

		if ( heldResources.Count == 0 )
		{
			if ( heldResource )
			{
				Destroy( heldResource );
			}

			inventoryBar.NullInventoryBar();
		}
		else if ( heldResources.Count > 0 )
		{
			SpawnResourceObject();
			inventoryBar.UpdateInventoryBar( resourceIndex, heldResources.ToArray() );
		}
	}

	void SpawnResourceObject()
	{
		inventoryBar.UpdateInventoryBar( resourceIndex, heldResources.ToArray() );

		if ( heldResource )
		{
			Destroy( heldResource );
		}

		if ( heldResources[resourceIndex].prefab )
		{
			heldResource = WadeUtils.Instantiate( heldResources[resourceIndex].prefab );
			heldResource.transform.parent = actor.GetBoneAtLocation( BoneLocation.RHand );
			WadeUtils.ResetTransform( heldResource.transform, true );
		}
	}

	void OnTriggerEnter( Collider other )
	{
		InventoryItem inventoryItem = other.gameObject.GetComponentInChildren<InventoryItem>();
		if ( inventoryItem && !inventoryItem.used )
		{
			inventoryItem.Use();

			PickupItem( inventoryItem.resourceData );

			WadeUtils.TempInstantiate( resourcePopPrefab, other.transform.position, Quaternion.identity, 1f );
		}
	}
}
