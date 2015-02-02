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
		DebugUtils.Assert( inventoryItemData.Length > 0 );

		NullInventoryBar();
		SetIcon( currentItemIcon, inventoryItemData[MathUtils.Mod( currentIndex, inventoryItemData.Length )].icon );

		if ( inventoryItemData.Length > 1 )
		{
			int prevIndex = MathUtils.Mod( ( currentIndex - 1 ), inventoryItemData.Length );
			SetIcon( prevItemIcon, inventoryItemData[prevIndex].icon );
		}

		if ( inventoryItemData.Length > 2 )
		{
			int nextIndex = MathUtils.Mod( ( currentIndex + 1 ), inventoryItemData.Length );
			SetIcon( nextItemIcon, inventoryItemData[nextIndex].icon );
		}
	}

	public void NullInventoryBar()
	{
		SetIcon( prevItemIcon );
		SetIcon( currentItemIcon );
		SetIcon( nextItemIcon );
	}

	void SetIcon( Image image, Sprite icon = null )
	{
		image.sprite = icon;
		image.color = Color.white;
	}
}
