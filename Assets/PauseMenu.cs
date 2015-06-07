using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	[SerializeField] KeyCode _pauseKey = KeyCode.Escape;

	[ReadOnly]
	[SerializeField] bool isPaused = false;

	Image _controlsImage = null;
	Text _controlsText = null;

	void Start()
	{
		_controlsImage = GetComponent<Image>();
		_controlsText = GetComponentInChildren<Text>();
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
		_controlsImage.enabled = true;
		_controlsText.enabled = true;
	}

	public void Unpause()
	{
		isPaused = false;
		Time.timeScale = 1f;
		_controlsImage.enabled = false;
		_controlsText.enabled = false;
	}
}
