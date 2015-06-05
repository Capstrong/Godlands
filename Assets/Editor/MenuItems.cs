using UnityEngine;
using UnityEditor;
using System.Collections;

public class MenuItems
{
	[MenuItem( "Assets/Create/Inventory Item" )]
	public static void CreateInventoryItem()
	{
		ScriptableObjectUtility.CreateAsset<InventoryItemData>();
	}

	[MenuItem( "Assets/Create/Resource Data" )]
	public static void CreateResourceData()
	{
		ScriptableObjectUtility.CreateAsset<ResourceData>();
	}

	[MenuItem( "Assets/Create/Buddy Item" )]
	public static void CreateBuddyItem()
	{
		ScriptableObjectUtility.CreateAsset<BuddyItemData>();
	}

	[MenuItem( "Assets/Create/Pickup Buddy Item" )]
	public static void CreatePickupBuddyItem()
	{
		ScriptableObjectUtility.CreateAsset<PickupBuddyItemData>();
	}

	[MenuItem( "Assets/Create/Travel Sound Player" )]
	public static void CreateTravelSoundPlayerAsset()
	{
		ScriptableObjectUtility.CreateAsset<TravelSoundPlayer>();
	}

	[MenuItem( "Assets/Create/Text Multi Volume Contents" )]
	public static void CreateTextMultiVolumeContents()
	{
		ScriptableObjectUtility.CreateAsset<TextMultiVolumeContents>();
	}

	[MenuItem( "Assets/Create/Time Render Settings Asset" )]
	public static void CreateTimeRenderSettingsAsset()
	{
		ScriptableObjectUtility.CreateAsset<TimeLightingSettingsData>();
	}
}
