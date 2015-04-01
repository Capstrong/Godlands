using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBox : MonoBehaviour {

	Text _UIText = null;

	void Start ()
	{
		_UIText = GetComponent<Text>();
	}
	
	public void SetTextForDuration( string textString, float duration = 5.0f )
	{
		_UIText.text = textString;
		CancelInvoke( "ClearText" );
		Invoke( "ClearText", duration );
	}

	public void ClearText()
	{
		_UIText.text = "";
	}
}
