using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBox : MonoBehaviour {

	Text _UIText = null;

	[SerializeField] KeyCode _clearKey = 0;

	void Start ()
	{
		_UIText = GetComponent<Text>();
	}

	void Update()
	{
		if ( Input.GetKeyDown( _clearKey ) )
		{
			ClearText();
			CancelInvoke( "ClearText" );
		}
	}

	public void SetText( string textString )
	{
		_UIText.text = textString;
		CancelInvoke( "ClearText" );
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

	public void ClearIfEqual( string textString )
	{
		if ( _UIText.text == textString )
		{
			ClearText();
		}
	}
}
