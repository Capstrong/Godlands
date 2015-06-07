using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	[SerializeField] KeyCode _pauseKey = KeyCode.Escape;

	[ReadOnly]
	[SerializeField] bool isPaused = false;

	Image _controlsImage = null;
	Text[] _controlsTexts = null;

	[SerializeField] float _fadeTime = 0f;
	[Tooltip( "The color that the overlay will be when it is in full effect" )]
	[SerializeField] Color _fullImageColor = Color.white;

	Coroutine _imageFadeRoutine = null;

	void Start()
	{
		_controlsImage = GetComponent<Image>();
		_controlsImage.color = Color.clear;
		_controlsTexts = GetComponentsInChildren<Text>();

		foreach ( Text text in _controlsTexts )
		{
			text.enabled = false;
		}
	}

	void Update()
	{
		if ( Input.GetKeyDown( _pauseKey ) )
		{
			if ( !isPaused )
			{
				Pause();
			}
			else
			{
				Unpause();
			}
		}
	}

	public void Pause()
	{
		isPaused = true;
		Time.timeScale = 0f;

		if ( _imageFadeRoutine != null )
		{
			StopCoroutine( _imageFadeRoutine );
		}

		_imageFadeRoutine = StartCoroutine( FadeImage( _controlsImage, _fullImageColor, _fadeTime ) );

		foreach ( Text text in _controlsTexts )
		{
			text.enabled = true;
		}
	}

	public void Unpause()
	{
		isPaused = false;
		Time.timeScale = 1f;
		
		if ( _imageFadeRoutine != null )
		{
			StopCoroutine( _imageFadeRoutine );
		}

		_imageFadeRoutine = StartCoroutine( FadeImage( _controlsImage, Color.clear, _fadeTime ) );
		
		foreach ( Text text in _controlsTexts )
		{
			text.enabled = false;
		}
	}

	IEnumerator FadeImage( Image image, Color endColor, float duration )
	{
		float startTime = Time.realtimeSinceStartup;
		Color startColor = image.color;

		while ( Time.realtimeSinceStartup < startTime + duration )
		{
			image.color = Color.Lerp( startColor, endColor, ( Time.realtimeSinceStartup - startTime ) / duration );

			yield return null;
		}
	}
}
