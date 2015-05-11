﻿using UnityEngine;
using System.Collections;

public class TextMultiVolume : TextVolume
{
	[SerializeField] TextMultiVolumeContents _textContents = null;

	public override void TriggerText()
	{
		if ( !_textContents.hasBeenDisplayed )
		{
			_textContents.hasBeenDisplayed = true;
			DisplayText( _textContents.text );
		}
	}
}