﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryScrollBar : MonoBehaviour
{
	[SerializeField] Image prevItemIcon = null;
	[SerializeField] Image currentItemIcon = null;
	[SerializeField] Image nextItemIcon = null;

	[SerializeField] Text prevItemCountText = null;
	[SerializeField] Text currentItemCountText = null;
	[SerializeField] Text nextItemCountText = null;

	[SerializeField] Color uiColor = Color.white;

	public void UpdateInventoryBar( int currentIndex, InventoryItemData[] inventoryItemData, InventoryDictionary inventory )
	{
		DebugUtils.Assert( inventoryItemData.Length > 0 );

		NullInventoryBar();
		SetIcon( currentItemIcon, inventoryItemData[MathUtils.Mod( currentIndex, inventoryItemData.Length )].icon );
		SetCountText( currentItemCountText, inventory[inventoryItemData[currentIndex]] );

		if ( inventoryItemData.Length > 1 )
		{
			int prevIndex = MathUtils.Mod( ( currentIndex - 1 ), inventoryItemData.Length );
			SetIcon( prevItemIcon, inventoryItemData[prevIndex].icon );
			SetCountText( prevItemCountText, inventory[inventoryItemData[prevIndex]] );
		}

		if ( inventoryItemData.Length > 2 )
		{
			int nextIndex = MathUtils.Mod( ( currentIndex + 1 ), inventoryItemData.Length );
			SetIcon( nextItemIcon, inventoryItemData[nextIndex].icon );
			SetCountText( nextItemCountText, inventory[inventoryItemData[nextIndex]] );
		}
	}

	public void NullInventoryBar()
	{
		SetIcon( prevItemIcon );
		SetIcon( currentItemIcon );
		SetIcon( nextItemIcon );

		SetCountText( prevItemCountText );
		SetCountText( currentItemCountText );
		SetCountText( nextItemCountText );
	}

	void SetIcon( Image image, Sprite icon = null )
	{
		image.sprite = icon;

		image.color = ( icon ? uiColor : image.color.SetAlpha( 0f ) );
	}

	void SetCountText( Text countText, int count = 0 )
	{
		countText.text = count.ToString();

		countText.color = ( count != 0 ? uiColor : countText.color.SetAlpha( 0f ) );
	}

	public void Disable()
	{
		gameObject.SetActive( false );
	}

	public void Enable()
	{
		gameObject.SetActive( true );
	}
}
