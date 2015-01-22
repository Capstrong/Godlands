using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorResources : ActorComponent 
{
	[SerializeField] GameObject inventoryBarPrefab;
	InventoryScrollBar inventoryBar;
	
	[SerializeField] ResourceData[] resourceTypes;

	Dictionary<ResourceData, int> heldResources = new Dictionary<ResourceData, int>();
	List<ResourceData> heldResourceTypes = new List<ResourceData>();

	int resourceIndex = 0;

	GameObject heldResource;

	// Use this for initialization
	void Start () 
	{
		foreach(ResourceData resourceData in resourceTypes)
		{
			heldResources.Add(resourceData, 0);
		}

		inventoryBar = GameObject.FindObjectOfType<InventoryScrollBar>();
		if(!inventoryBar)
		{
			GameObject ibgo = WadeUtils.Instantiate(inventoryBarPrefab);
			inventoryBar = ibgo.GetComponent<InventoryScrollBar>();
		}

		// Not ready for this yet
		// PickupResource(resourceTypes[0]);

		if(heldResourceTypes.Count > 0)
		{
			SpawnResourceObject();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		float scrollAmount = Input.GetAxis("Scroll" + WadeUtils.platformName);
		if((scrollAmount > WadeUtils.SMALLNUMBER || scrollAmount < -WadeUtils.SMALLNUMBER) & heldResourceTypes.Count > 0)
		{
			int nextIndex = resourceIndex + Mathf.CeilToInt(Mathf.Clamp(scrollAmount, -1f, 1f));
			int numResources = heldResourceTypes.Count;

			// keep within bounds
			if(nextIndex > numResources - 1)
			{
				nextIndex = nextIndex % numResources;
			}
			else if(nextIndex < 0)
			{
				nextIndex = numResources - ((-nextIndex) % numResources);
			}
			
			resourceIndex = Mathf.FloorToInt(Mathf.Clamp(nextIndex, 0, heldResources.Count - 1));
			SpawnResourceObject();
		}
	}

	void PickupResource(ResourceData addedResource)
	{
		heldResources[addedResource]++;
		heldResourceTypes.Clear();

		foreach(ResourceData resourceData in resourceTypes)
		{
			if(heldResources[resourceData] > 0)
			{
				heldResourceTypes.Add(resourceData);
			}
		}
	}

	void SpawnResourceObject()
	{
		inventoryBar.UpdateInventoryBar(resourceIndex, heldResourceTypes.ToArray());

		if(heldResource)
		{
			Destroy(heldResource);
		}

		if(heldResourceTypes[resourceIndex].prefab)
		{
			heldResource = WadeUtils.Instantiate(heldResourceTypes[resourceIndex].prefab);
			heldResource.transform.parent = actor.GetBoneAtLocation(BoneLocation.RHand);
			WadeUtils.ResetTransform(heldResource.transform, true);
		}
	}
}
