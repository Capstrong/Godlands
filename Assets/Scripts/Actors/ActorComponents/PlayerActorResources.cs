using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof( ActorCamera ) )]
public class PlayerActorResources : ActorComponent
{
	[SerializeField] GameObject inventoryBarPrefab = null;
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
	List<InventoryItemData> heldResourceTypes = new List<InventoryItemData>();

	int resourceIndex = 0;
	GameObject heldResource;

	private ActorCamera _actorCamera;

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
		if ( !inventoryBar )
		{
			GameObject ibgo = WadeUtils.Instantiate( inventoryBarPrefab );
			inventoryBar = ibgo.GetComponent<InventoryScrollBar>();
		}

		if ( heldResourceTypes.Count > 0 )
		{
			SpawnResourceObject();
		}
	}

	// Update is called once per frame
	void Update()
	{
		CheckScroll();
		CheckGiveResource();
	}

	void CheckScroll()
	{
		float scrollAmount = Input.GetAxis( "Scroll" + WadeUtils.platformName );
		if ( ( scrollAmount > WadeUtils.SMALLNUMBER || scrollAmount < -WadeUtils.SMALLNUMBER ) & heldResourceTypes.Count > 0 )
		{
			// Need to do this so >0 rounds up and <0 rounds down
			int nextIndex = resourceIndex + scrollAmount > 0f ? Mathf.CeilToInt( Mathf.Clamp( scrollAmount, -1f, 1f ) ) :
			                                                    Mathf.FloorToInt( Mathf.Clamp( scrollAmount, -1f, 1f ) );
			int numResources = heldResourceTypes.Count;

			// keep within bounds
			if ( nextIndex > numResources - 1 )
			{
				nextIndex = nextIndex % numResources;
			}
			else if ( nextIndex < 0 )
			{
				nextIndex = numResources - ( ( -nextIndex ) % numResources );
			}

			resourceIndex = Mathf.FloorToInt( Mathf.Clamp( nextIndex, 0, heldResourceTypes.Count - 1 ) );
			SpawnResourceObject();
		}
	}

	void CheckGiveResource()
	{
		if ( Input.GetMouseButtonDown( 0 ) &&
		     heldResourceTypes.Count > 0 &&
		     heldResourceTypes[resourceIndex] is ResourceData )
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
	}

	void GiveResource( BuddyStats buddyStats )
	{
		buddyStats.GiveResource( actor.actorPhysics, (ResourceData)heldResourceTypes[resourceIndex] );
		inventory[heldResourceTypes[resourceIndex]]--;

		UpdateResourceList();
	}

	void PickupResource( InventoryItemData itemData )
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
		heldResourceTypes.Clear();

		foreach ( InventoryItemData itemData in heldResourceTypes )
		{
			heldResourceTypes.Add( itemData );
		}

		if ( heldResourceTypes.Count == 0 )
		{
			if ( heldResource )
			{
				Destroy( heldResource );
			}

			inventoryBar.NullInventoryBar();
		}
		else if ( heldResourceTypes.Count == 1 )
		{
			SpawnResourceObject();
		}
	}

	void SpawnResourceObject()
	{
		inventoryBar.UpdateInventoryBar( resourceIndex, heldResourceTypes.ToArray() );

		if ( heldResource )
		{
			Destroy( heldResource );
		}

		if ( heldResourceTypes[resourceIndex].prefab )
		{
			heldResource = WadeUtils.Instantiate( heldResourceTypes[resourceIndex].prefab );
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

			PickupResource( inventoryItem.resourceData );

			Destroy( other.gameObject );
			WadeUtils.TempInstantiate( resourcePopPrefab, other.transform.position, Quaternion.identity, 1f );
		}
	}
}
