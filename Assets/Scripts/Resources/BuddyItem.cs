using UnityEngine;
using System.Collections;

public class BuddyItem : InventoryItem
{
	[SerializeField] GameObject _internals = null;
	[SerializeField] TextMultiVolumeContents _textContents = null;
	[SerializeField] float _timeUntilFadeout = 7.0f;
	[SerializeField] float _fadeoutDuration = 2.0f;

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
			FindObjectOfType<TextBox>().SetTextForDuration( _textContents.text, _timeUntilFadeout, _fadeoutDuration );
		}
	}
}
