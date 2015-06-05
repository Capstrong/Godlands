using UnityEngine;
using System.Collections.Generic;

public class MultiTextMultiVolume : TextVolume
{
	[SerializeField] List<TextMultiVolumeContents> _textContents = null;

	public override void TriggerText( PlayerActor player )
	{
		foreach ( TextMultiVolumeContents text in _textContents )
		{
			if ( !text.hasBeenDisplayed )
			{
				text.hasBeenDisplayed = true;
				DisplayText( text.text );
				Reactivate();
			}
		}
	}
}
