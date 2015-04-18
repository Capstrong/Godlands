using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBox : MonoBehaviour {

	Text _UIText = null;

	[SerializeField] KeyCode _clearKey = 0;

	Coroutine _textFade;

	void Start ()
	{
		_UIText = GetComponent<Text>();
	}

	void Update()
	{
		if ( Input.GetKeyDown( _clearKey ) )
		{
			ClearText();
		}
	}

	public void SetText( string textString )
	{
		ClearText();

		_UIText.text = textString;
	}
	
	public void SetTextForDuration( string textString, float duration = 5.0f, float fadeoutDuration = 2.0f )
	{
		ClearText();

		_UIText.text = textString;
		_textFade = StartCoroutine( Fadeout( duration, fadeoutDuration ) );
	}

	public void ClearText()
	{
		_UIText.text = "";

		if ( _textFade != null )
		{
			StopCoroutine( _textFade );
		}
	}

	IEnumerator Fadeout( float delay, float duration )
	{
		// Initially wait for the amount of time needed until text should fade.
		yield return new WaitForSeconds( delay );

		// Perform text fade.
		do
		{
			yield return null;

			duration -= Time.deltaTime;
		}
		while ( duration > 0.0f );

		// Clear the text.
		_UIText.text = "";
	}
}
