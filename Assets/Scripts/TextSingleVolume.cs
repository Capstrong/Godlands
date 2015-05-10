using UnityEngine;
using System.Collections;

public class TextSingleVolume : TextVolume
{
	[TextArea( 3, 10 )]
	[SerializeField] string _text = "";
	public string text
	{
		get { return _text; }
	}

	public override void TriggerText()
	{
		DisplayText( text );
	}
}
