using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryScrollBar : MonoBehaviour
{
	[SerializeField] Image prevItemIcon = null;
	[SerializeField] Image currentItemIcon = null;
	[SerializeField] Image nextItemIcon = null;

	public void UpdateInventoryBar( int currentIndex, InventoryItemData[] inventoryItemData )
	{
		SetIcon( currentItemIcon, inventoryItemData[currentIndex].icon );
		// tell player to hold item

		int prevIndex = currentIndex - 1;
		if ( prevIndex < 0 )
		{
			prevIndex = inventoryItemData.Length - 1;
		}
		SetIcon( prevItemIcon, inventoryItemData[prevIndex].icon );

		int nextIndex = currentIndex + 1;
		if ( nextIndex > inventoryItemData.Length - 1 )
		{
			nextIndex = 0;
		}
		SetIcon( nextItemIcon, inventoryItemData[nextIndex].icon );
	}

	public void NullInventoryBar()
	{
		SetIcon( prevItemIcon );
		SetIcon( currentItemIcon );
		SetIcon( nextItemIcon );
	}

	void SetIcon( Image image, Sprite icon = null )
	{
		if ( icon )
		{
			image.sprite = icon;
			image.color = Color.white;
		}
		else
		{
			image.color = Color.white * 0f;
		}
	}
}
