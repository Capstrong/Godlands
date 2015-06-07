using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	[SerializeField] KeyCode _pauseKey = KeyCode.Escape;

	[ReadOnly]
	[SerializeField] bool isPaused = false;

	Image _overlayImage = null;
	Text[] _controlsTexts = null;
	Image[] _controlsImages = null;
	UnityEngine.UI.Button _mainMenuButton = null;

	[SerializeField] float _fadeTime = 0f;
	[Tooltip( "The color that the overlay will be when it is in full effect" )]
	[SerializeField] Color _fullImageColor = Color.white;

	Coroutine _imageFadeRoutine = null;

	void Start()
	{
		_overlayImage = GetComponent<Image>();
		_overlayImage.color = Color.clear;

		_controlsImages = GetComponentsInChildren<Image>();

		foreach ( Image image in _controlsImages )
		{
			image.enabled = false;
		}

		_controlsTexts = GetComponentsInChildren<Text>();

		foreach ( Text text in _controlsTexts )
		{
			text.enabled = false;
		}

		_mainMenuButton = GetComponentInChildren<UnityEngine.UI.Button>();
		_mainMenuButton.enabled = false;
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

		_imageFadeRoutine = StartCoroutine( FadeImage( _overlayImage, _fullImageColor, _fadeTime ) );

		foreach ( Image image in _controlsImages )
		{
			image.enabled = true;
		}


		foreach ( Text text in _controlsTexts )
		{
			text.enabled = true;
		}

		_mainMenuButton.enabled = true;

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void Unpause()
	{
		isPaused = false;
		Time.timeScale = 1f;
		
		if ( _imageFadeRoutine != null )
		{
			StopCoroutine( _imageFadeRoutine );
		}

		_imageFadeRoutine = StartCoroutine( FadeImage( _overlayImage, Color.clear, _fadeTime ) );
		
		foreach ( Image image in _controlsImages )
		{
			image.enabled = false;
		}


		foreach ( Text text in _controlsTexts )
		{
			text.enabled = false;
		}

		_mainMenuButton.enabled = false;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
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

	void OnDestroy()
	{
		Time.timeScale = 1f;
	}
}
