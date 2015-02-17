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

	[SerializeField] LayerMask buddyLayer = 0;
	[SerializeField] float maxGiveDistance = 2f;

	public override void Awake()
	{
		base.Awake();
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
		                                                  (actor as PlayerActor).actorCamera.cam.transform.forward,
		                                                  buddyLayer,
		                                                  maxGiveDistance );

		if ( hitInfo.transform )
		{
			BuddyStats buddyStats = hitInfo.transform.GetComponent<BuddyStats>();

			GodTag godTag = GetComponent<GodTag>(); // For checking if this actor owns the buddy

			if ( buddyStats &&
			     buddyStats.isAlive &&
			     ( buddyStats.owner == null || buddyStats.owner == godTag ) )
			{
				buddyStats.owner = godTag;
				GiveResource( buddyStats );
			}
		}
	}

	void GiveResource( BuddyStats buddyStats )
	{
		buddyStats.GiveResource( (actor as PlayerActor).actorStats, (ResourceData)heldResources[resourceIndex] );
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

		MeshRenderer[] childRenderers = newBuddy.gameObject.GetComponentsInChildren<MeshRenderer>();

		foreach ( MeshRenderer meshRenderer in childRenderers )
		{
			if ( meshRenderer.gameObject.name == "Body" )
			{
				// buddyItemData.prefab.GetComponentInChildren<MeshRenderer>() was not working
				MeshRenderer render = buddyItemData.prefab.transform.FindChild( "Sphere" ).gameObject.GetComponent<MeshRenderer>();
				meshRenderer.material = render.material;
			}
		}

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
