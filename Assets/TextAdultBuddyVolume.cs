using UnityEngine;
using System.Collections;

public class TextAdultBuddyVolume : TextVolume
{
	[TextArea( 3, 10 ), Tooltip( "Text to show if the player is carrying an adult buddy" )]
	[SerializeField] string _adultBuddyText = "";

	[TextArea( 3, 10 ), Tooltip( "Text to show if the player is NOT carrying an adult buddy" )]
	[SerializeField] string _noAdultBuddyText = "";

	public override void TriggerText( PlayerActor player )
	{
		if ( player
			&& player.inventory
			&& player.inventory.backBuddy
			&& player.inventory.backBuddy.hiddenBuddy
			&& player.inventory.backBuddy.hiddenBuddy.isOfAge )
		{
			DisplayText( _adultBuddyText );
			Reactivate();
		}
		else
		{
			DisplayText( _noAdultBuddyText );
			Reactivate();
		}
	}
}
