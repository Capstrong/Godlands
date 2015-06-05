using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
	Text _UIText = null;
	[SerializeField] Image _backgroundImage = null;

	[Range( 0.0f, 1.0f )]
	[SerializeField] float _maxBackgroundOpacity = 0.75f;

	[SerializeField] float _textDisplayDuration = 7.0f;
	[SerializeField] float _textFadeOutDuration = 1.0f;

	Queue<string> _textQueue = new Queue<string>();
	bool _isTextDisplayed = false;
	Coroutine _textFade;

	void Awake()
	{
		_UIText = GetComponent<Text>();
	}

	public void SetText( string textString )
	{
		_textQueue.Enqueue( textString );

		if ( !_isTextDisplayed )
		{
			DisplayNext();
		}
	}

	public void OverrideText( string textString )
	{
		_textQueue.Clear();
		_textQueue.Enqueue( textString );
		CancelInvoke( "DisplayNext" );
		DisplayNext();
	}

	void DisplayNext()
	{
		if ( _textFade != null )
		{
			StopCoroutine( _textFade );
		}

		if ( _textQueue.Count > 0 )
		{
			_UIText.text = "";
			_UIText.color = _UIText.color.SetAlpha( 1.0f );
			_backgroundImage.color = _backgroundImage.color.SetAlpha( 1.0f );
			_UIText.text = _textQueue.Dequeue();

			Invoke( "DisplayNext", _textDisplayDuration );
			_isTextDisplayed = true;
		}
		else
		{
			_textFade = StartCoroutine( Fadeout( _textFadeOutDuration ) );
			_isTextDisplayed = false;
		}
	}

	IEnumerator Fadeout( float duration )
	{
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
	}
}
