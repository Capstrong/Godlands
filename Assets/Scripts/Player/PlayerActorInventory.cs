using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof( ActorCamera ) )]
public class PlayerActorInventory : ActorComponent
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
	Dictionary<InventoryItemData, int> inventory = new Dictionary<InventoryItemData, int>();

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
		CheckScroll();
	}

	void CheckScroll()
	{
		float scrollAmount = Input.GetAxis( "Scroll" + PlatformUtils.platformName );
		if ( ( scrollAmount > WadeUtils.SMALLNUMBER || scrollAmount < -WadeUtils.SMALLNUMBER ) & heldResources.Count > 0 )
		{
			// Need to do this so >0 rounds up and <0 rounds down
			int nextIndex = resourceIndex + Mathf.Clamp( Mathf.RoundToInt( scrollAmount ), -1, 1 );
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
		BuddyItemData buddyItemData = (BuddyItemData)heldResources[resourceIndex];
		BuddyStats newBuddy = ( Instantiate( buddyItemData.buddyPrefab,
		                                     transform.position + transform.forward * 3.0f,
		                                     Quaternion.identity ) as GameObject ).GetComponent<BuddyStats>();
		newBuddy.owner = GetComponent<GodTag>();
		newBuddy.statType = buddyItemData.stat;

		// this could be bad, should probably run it by Chris
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
			inventoryItem.used = true;

			PickupItem( inventoryItem.resourceData );

			Destroy( other.gameObject );
			WadeUtils.TempInstantiate( resourcePopPrefab, other.transform.position, Quaternion.identity, 1f );
		}
	}
}
