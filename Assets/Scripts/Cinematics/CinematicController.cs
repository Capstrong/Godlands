using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CinematicController : MonoBehaviour 
{
	[SerializeField] bool _playIntroCinematic = true;

	Transform _transform = null;

	Canvas[] _uiCanvases = null;
	Animator _cutsceneAnimator = null;
	GameObject _playerObj = null;
	
	[SerializeField] AudioSource _cinematicDialoguePrefab = null;
	AudioSource _cinematicDialogue = null;

	void Awake()
	{
		_transform = GetComponent<Transform>();
		_cutsceneAnimator = GetComponent<Animator>();
	}

	void Start()
	{
		if( _playIntroCinematic )
		{
			PlayerActor playerActor = GameObject.FindObjectOfType<PlayerActor>();
			if( playerActor )
			{
				_playerObj = playerActor.gameObject;
				_playerObj.SetActive( false );
			}
			else
			{
				Debug.LogWarning( "No player to set inactive during cutscene." );
			}

			_uiCanvases = GameObject.FindObjectsOfType<Canvas>();
			foreach( Canvas canvas in _uiCanvases )
			{
				canvas.enabled = false;
			}

			_cutsceneAnimator.enabled = true;
			_cutsceneAnimator.Play( "EarthToGodlandsShot" );

			_cinematicDialogue = SoundManager.Play2DSound( _cinematicDialoguePrefab );
		}
	}

	void Update()
	{
		if( Input.GetButtonDown( "Jump" + PlatformUtils.platformName ) )
		{
			Debug.Log("EndCutscene");
			SkipCutscene();
		}
	}

	void SkipCutscene()
	{
		FinishCutscene();
	}

	public void SetToNearestRenderSettings()
	{
		RenderSettingsManager.SetToNearestZone( _transform.position );
	}

	public void TransitionToNearestRenderSettings( float transitionTime )
	{
		RenderSettingsManager.TransitionToNearestZone( _transform.position, transitionTime );
	}

	public void FinishCutscene()
	{
		if( _playerObj )
		{
			_playerObj.SetActive( true );
		}
		else
		{
			Debug.LogWarning( "No player to set active after cutscene" );
		}

		if( _uiCanvases.Length > 0 )
		{
			foreach( Canvas canvas in _uiCanvases )
			{
				canvas.enabled = true;
			}
		}

		if( _cinematicDialogue )
		{
			_cinematicDialogue.Stop();
		}

		_cutsceneAnimator.StopPlayback();
		_cutsceneAnimator.enabled = false;
	}
}
