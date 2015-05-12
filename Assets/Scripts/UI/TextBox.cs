using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBox : MonoBehaviour {

	Text _UIText = null;
	[SerializeField] Image _backgroundImage = null;

	[SerializeField] KeyCode _clearKey = 0;
	[Range( 0.0f, 1.0f )]
	[SerializeField] float _maxBackgroundOpacity = 0.75f;

	[SerializeField] float _textDisplayDuration = 7.0f;
	[SerializeField] float _textFadeOutDuration = 1.0f;

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
		_UIText.color = _UIText.color.SetAlpha( 1.0f );
		_backgroundImage.color = _backgroundImage.color.SetAlpha( _maxBackgroundOpacity );
		_UIText.text = textString;

		_textFade = StartCoroutine( Fadeout( _textDisplayDuration, _textFadeOutDuration ) );
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

		float elapsedTime = 0.0f;

		// Perform text fade.
		do
		{
			yield return null;
			elapsedTime += Time.deltaTime;

			_UIText.color = _UIText.color.SetAlpha( Mathf.Lerp( 1.0f, 0.0f, elapsedTime / duration ) );
			_backgroundImage.color = _backgroundImage.color.SetAlpha( Mathf.Lerp( _maxBackgroundOpacity, 0.0f, elapsedTime / duration ) );
		}
		while ( elapsedTime < duration );

		// Clear the text.
		_textFade = null;
		ClearText();
	}
}
