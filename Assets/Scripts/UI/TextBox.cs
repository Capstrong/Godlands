using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBox : MonoBehaviour {

	Text _UIText = null;
	[SerializeField] Image _backgroundImage = null;

	[SerializeField] KeyCode _clearKey = 0;
	[Range( 0.0f, 1.0f )]
	[SerializeField] float _maxBackgroundOpacity = 0.75f;

	Coroutine _textFade;

	void Awake()
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
		_backgroundImage.color = _backgroundImage.color.SetAlpha( _maxBackgroundOpacity );
		_UIText.text = textString;
	}
	
	public void SetTextForDuration( string textString, float duration = 5.0f, float fadeoutDuration = 2.0f )
	{
		SetText( textString );
		_textFade = StartCoroutine( Fadeout( duration, fadeoutDuration ) );
	}

	public void ClearText()
	{
		_UIText.text = "";
		_backgroundImage.color = _backgroundImage.color.SetAlpha( 0.0f );
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
		_textFade = null;
		ClearText();
	}
}
