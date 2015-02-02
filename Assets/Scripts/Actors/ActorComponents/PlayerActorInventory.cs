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

	// types and current count
	Dictionary<InventoryItemData, int> inventory = new Dictionary<InventoryItemData, int>();

	// currently held types to show on UI bar
	List<InventoryItemData> heldResources = new List<InventoryItemData>();

	int resourceIndex = 0;
	GameObject heldResource;

	ActorCamera _actorCamera;

	[SerializeField] LayerMask buddyLayer = 0;
	[SerializeField] float maxGiveDistance = 2f;

	public override void Awake()
	{
		base.Awake();

		_actorCamera = GetComponent<ActorCamera>();
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
		CheckUseItem();
	}

	void CheckScroll()
	{
		float scrollAmount = Input.GetAxis( "Scroll" + WadeUtils.platformName );
		if ( ( scrollAmount > WadeUtils.SMALLNUMBER || scrollAmount < -WadeUtils.SMALLNUMBER ) & heldResources.Count > 0 )
		{
			// Need to do this so >0 rounds up and <0 rounds down
			int nextIndex = resourceIndex + Mathf.Clamp( Mathf.RoundToInt( scrollAmount ), -1, 1 );
			resourceIndex = MathUtils.Mod( nextIndex, heldResources.Count );
			SpawnResourceObject();
		}
	}

	void CheckUseItem()
	{
		if ( Input.GetMouseButtonDown( 0 ) &&
		     heldResources.Count > 0 )
		{
			if ( heldResources[resourceIndex] is ResourceData )
			{
				CheckGiveResources();
			}
			else
			{
				SpawnBuddy();
			}
		}
	}

	void CheckGiveResources()
	{
		RaycastHit hitInfo = WadeUtils.RaycastAndGetInfo( transform.position,
		                                                  _actorCamera.cam.transform.forward,
		                                                  buddyLayer,
		                                                  maxGiveDistance );

		if ( hitInfo.transform )
		{
			BuddyStats buddyStats = hitInfo.transform.GetComponent<BuddyStats>();
			if ( buddyStats &&
			     buddyStats.isAlive &&
			     ( buddyStats.owner == null || buddyStats.owner == GetComponent<GodTag>() ) )
			{
				GiveResource( buddyStats );
			}
		}
	}

	void GiveResource( BuddyStats buddyStats )
	{
		buddyStats.GiveResource( actor.actorPhysics, (ResourceData)heldResources[resourceIndex] );
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
		InventoryItem inventoryItem = other.gameObject.GetComponent<InventoryItem>();
		if ( inventoryItem && !inventoryItem.used )
		{
			inventoryItem.used = true;

			PickupItem( inventoryItem.resourceData );

			Destroy( other.gameObject );
			WadeUtils.TempInstantiate( resourcePopPrefab, other.transform.position, Quaternion.identity, 1f );
		}
	}
}
