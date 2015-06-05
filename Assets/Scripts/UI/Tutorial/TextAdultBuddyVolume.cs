using UnityEngine;
using System.Collections;

public class TextAdultBuddyVolume : TextVolume
{
	[TextArea( 3, 10 ), Tooltip( "Text to show if the player is carrying an adult buddy" )]
	[SerializeField] string _adultBuddyText = "";

	[TextArea( 3, 10 ), Tooltip( "Text to show if the player is NOT carrying an adult buddy" )]
	[SerializeField] string _noAdultBuddyText = "";

	public float activationDelay = 10.0f;
	bool _canActivate = true;

	public override void TriggerText( PlayerActor player )
	{
		if ( player && _canActivate )
		{
			if ( player.inventory.backBuddy
				&& player.inventory.backBuddy.hiddenBuddy
				&& player.inventory.backBuddy.hiddenBuddy.isOfAge )
			{
				DisplayText( _adultBuddyText );
				Reactivate();
				_canActivate = false;
				Invoke( "Enable", activationDelay );
			}
			else
			{
				DisplayText( _noAdultBuddyText );
				Reactivate();
				_canActivate = false;
				Invoke( "Enable", activationDelay );
			}
		}
	}

	void Enable()
	{
		_canActivate = true;
	}
}
