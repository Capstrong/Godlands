using UnityEngine;
using System;
using System.Collections.Generic;

public class MultiTextVolume : TextVolume
{
	[Serializable]
	public struct TextContents
	{
		[TextArea( 3, 10 )]
		public string text;
	}

	[SerializeField] List<TextContents> _textContents = null;
	bool _hasBeenDisplayed = false;

	public override void TriggerText( PlayerActor player )
	{
		if ( !_hasBeenDisplayed )
		{ 
			foreach ( TextContents text in _textContents )
			{
				GameObject.FindObjectOfType<TextBox>().SetText( text.text );
			}
			_hasBeenDisplayed = true;
		}
	}
}
