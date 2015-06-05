using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryScrollBar : MonoBehaviour
{
	[SerializeField] Color uiColor = Color.white;

	[SerializeField] Image prevItemIcon = null;
	[SerializeField] Image currentItemIcon = null;
	[SerializeField] Image nextItemIcon = null;

	[SerializeField] Text prevItemCountText = null;
	[SerializeField] Text currentItemCountText = null;
	[SerializeField] Text nextItemCountText = null;

	[SerializeField] InventoryArrow _leftArrow = null;
	[SerializeField] InventoryArrow _rightArrow = null;

	[SerializeField] int _highlightFrameCount = 0;

	Coroutine _highlightRoutine = null;
	Animator _currentIconAnimator = null;
	Animator currentIconAnimator
	{
		get
		{
			if( !_currentIconAnimator )
			{
				_currentIconAnimator = currentItemIcon.GetComponent<Animator>();
			}

			return _currentIconAnimator;
		}
	}

	public void UpdateInventoryBar( PlayerActor player, int currentIndex, InventoryItemData[] inventoryItemData, InventoryDictionary inventory )
	{
		DebugUtils.Assert( inventoryItemData.Length > 0 );

		NullInventoryBar();
		SetIcon( currentItemIcon, inventoryItemData[MathUtils.Mod( currentIndex, inventoryItemData.Length )].icon );

		int resourceCount = inventoryItemData[currentIndex].showNumber ? inventory[inventoryItemData[currentIndex]] : -1;
		SetCountText( currentItemCountText, resourceCount );

		bool animateIcon = ( inventoryItemData[currentIndex] is BuddyItemData || ( inventoryItemData[currentIndex] is PickupBuddyItemData ) ) &&
						   inventoryItemData[currentIndex].CanUseItem( player, new RaycastHit() );
		currentIconAnimator.SetBool( "CanUse", animateIcon );

		if ( inventoryItemData.Length > 1 )
		{
			int prevIndex = MathUtils.Mod( ( currentIndex - 1 ), inventoryItemData.Length );
			SetIcon( prevItemIcon, inventoryItemData[prevIndex].icon );

			resourceCount = inventoryItemData[prevIndex].showNumber ? inventory[inventoryItemData[prevIndex]] : -1;
			SetCountText( prevItemCountText, resourceCount );
		}

		if ( inventoryItemData.Length > 2 )
		{
			int nextIndex = MathUtils.Mod( ( currentIndex + 1 ), inventoryItemData.Length );
			SetIcon( nextItemIcon, inventoryItemData[nextIndex].icon );

			resourceCount = inventoryItemData[nextIndex].showNumber ? inventory[inventoryItemData[nextIndex]] : -1;
			SetCountText( nextItemCountText, resourceCount );
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
		if( count == -1 )
		{
			countText.text = string.Empty;
		}
		else
		{
			countText.text = count.ToString();
		}

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

	public void UpdateScrollArrows( float scrollAmount )
	{
		if ( WadeUtils.IsZero( scrollAmount ) )
		{
			if ( _highlightRoutine == null )
			{
				UnhighlightArrows();
			}
		}
		else
		{
			if ( _highlightRoutine != null )
			{
				StopCoroutine( _highlightRoutine );
			}

			_highlightRoutine = StartCoroutine( HighlightArrowRoutine( scrollAmount > 0f ) );
		}
	}

	public IEnumerator HighlightArrowRoutine( bool highlightLeft )
	{
		InventoryArrow highlightArrow = ( highlightLeft ? _leftArrow : _rightArrow );
		InventoryArrow normalArrow = ( highlightLeft ? _rightArrow : _leftArrow );

		for ( int i = 0; i < _highlightFrameCount; i++ )
		{
			highlightArrow.Highlight();
			normalArrow.Normal();
			yield return null;
		}

		_highlightRoutine = null;
	}

	public void UnhighlightArrows()
	{
		_rightArrow.Normal();
		_leftArrow.Normal();
	}
}
