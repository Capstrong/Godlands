﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof( ActorCamera ) )]
public class PlayerActorResources : ActorComponent
{
	[SerializeField] GameObject inventoryBarPrefab = null;
	InventoryScrollBar inventoryBar;

	[SerializeField] AudioSource resourcePopAudio = null;

	// possible types to get (might want to instead load this from folder)
	[SerializeField] ResourceData[] resourceTypes = null;

	// types and current count
	Dictionary<ResourceData, int> resourceTypeCounts = new Dictionary<ResourceData, int>();

	// currently held types to show on UI bar
	List<ResourceData> heldResourceTypes = new List<ResourceData>();

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
		foreach ( ResourceData resourceData in resourceTypes )
		{
			resourceTypeCounts.Add( resourceData, 0 );
		}

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
		if((scrollAmount > WadeUtils.SMALLNUMBER || scrollAmount < -WadeUtils.SMALLNUMBER) & heldResourceTypes.Count > 0)
		{
			// Need to do this so >0 rounds up and <0 rounds down
			int nextIndex = resourceIndex + Mathf.Clamp( Mathf.RoundToInt( scrollAmount ), -1, 1 );
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

			resourceIndex = Mathf.Clamp( nextIndex, 0, heldResourceTypes.Count - 1 );
			SpawnResourceObject();
		}
	}

	void CheckGiveResource()
	{
		if ( Input.GetMouseButtonDown( 0 ) && heldResourceTypes.Count > 0 )
		{
			RaycastHit hitInfo = WadeUtils.RaycastAndGetInfo( transform.position,
			                                                  _actorCamera.cam.transform.forward,
			                                                  buddyLayer,
			                                                  maxGiveDistance );

			if ( hitInfo.transform )
			{
				BuddyStats buddyStats = hitInfo.transform.GetComponent<BuddyStats>();
				if ( buddyStats && buddyStats.isAlive )
				{
					GiveResource( buddyStats );
				}
			}
		}
	}

	void GiveResource( BuddyStats buddyStats )
	{
		buddyStats.GiveResource( actor.actorPhysics, heldResourceTypes[resourceIndex] );
		resourceTypeCounts[heldResourceTypes[resourceIndex]]--;

		UpdateResourceList();
	}

	void PickupResource( ResourceData addedResource )
	{
		resourceTypeCounts[addedResource]++;
		UpdateResourceList();
	}

	void UpdateResourceList()
	{
		heldResourceTypes.Clear();

		foreach ( ResourceData resourceData in resourceTypes )
		{
			if ( resourceTypeCounts[resourceData] > 0 )
			{
				heldResourceTypes.Add( resourceData );
			}
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
		Resource resourceComponent = other.gameObject.GetComponent<Resource>();
		if ( resourceComponent && !resourceComponent.used )
		{
			resourceComponent.used = true;

			PickupResource( resourceComponent.resourceData );

			Destroy( other.gameObject );
			SoundManager.instance.Play3DSoundAtPosition( resourcePopAudio, other.transform.position );
		}
	}
}
