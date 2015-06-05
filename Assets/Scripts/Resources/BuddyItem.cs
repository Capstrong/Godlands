using UnityEngine;
using System.Collections;

public class BuddyItem : InventoryPickupItem
{
	[SerializeField] GameObject _internals = null;
	[SerializeField] TextMultiVolumeContents _textContents = null;

	public override void Start()
	{
		base.Start();

		BuddyItemData buddyData = Instantiate<BuddyItemData>( (BuddyItemData) resourceData );
		buddyData.respawnItem = this;

		resourceData = buddyData;
	}

	public override void Use()
	{
		base.Use();

		_internals.SetActive( false );

		if ( !_textContents.hasBeenDisplayed )
		{
			_textContents.hasBeenDisplayed = true;
			FindObjectOfType<TextBox>().SetText( _textContents.text );
		}
	}

	public override void Enable()
	{
		base.Enable();

		_internals.SetActive( true );
	}
}
