using UnityEngine;
using UnityEditor;
using System.Collections;

public class MenuItems
{
	[MenuItem("Assets/Create/Inventory Item")]
	public static void CreateInventoryItem()
	{
		ScriptableObjectUtility.CreateAsset<InventoryItemData>();
	}

	[MenuItem("Assets/Create/Resource Data")]
	public static void CreateResourceData()
	{
		ScriptableObjectUtility.CreateAsset<ResourceData>();
	}

	[MenuItem( "Assets/Create/Buddy Item" )]
	public static void CreateBuddyItem()
	{
		ScriptableObjectUtility.CreateAsset<BuddyItemData>();
	}
}
