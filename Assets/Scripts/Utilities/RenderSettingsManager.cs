using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class TimeRenderSettings
{
	public Color skyColor = Color.white;

	public Color lightColor = Color.white;
	public float lightIntensity = 0.5f;

	public Color fogColor = Color.white;
	public float fogDensity = 0.05f;
}

[Serializable]
public struct RenderSettingsData
{
	public TimeRenderSettings daySettings;
	public TimeRenderSettings nightSettings;

	public Material skyMaterial;
}

[ExecuteInEditMode]
public class RenderSettingsManager : SingletonBehaviour<RenderSettingsManager> 
{
	[SerializeField] RenderSettingsData _currentRenderSettings = new RenderSettingsData();
	RenderSettingsData _targetRenderSettings = new RenderSettingsData();

	[SerializeField] Light _dirLight = null;
	Material _curSkyboxMaterial = null;
	int _curSkyboxTintPropertyID = 0;

	[Tooltip("The length (in seconds) of a day")]
	[SerializeField] float _dayCycleTime = 60;
	[SerializeField] float _dayCycleTimer = 0f;

	[Tooltip("Zero is noon, 1 is midnight")]
	[Range(0, 1)]
	public float timeOfDay; // This should not go in here. Eventually it will be with the Day Cycle

	void Awake()
	{
		_curSkyboxMaterial = _currentRenderSettings.skyMaterial;
		_curSkyboxTintPropertyID = Shader.PropertyToID( "_Tint" );
	}

	void Update()
	{
		_dayCycleTimer += Time.deltaTime;

		timeOfDay = ( -Mathf.Cos( _dayCycleTimer/_dayCycleTime * 2f * Mathf.PI ) * 0.5f ) + 0.5f;

		if( _dayCycleTimer > _dayCycleTime )
		{
			_dayCycleTimer -= _dayCycleTime;
		}
		else if( _dayCycleTimer < 0f )
		{
			_dayCycleTimer += _dayCycleTime;
		}

		ApplyRenderSettings();
	}

	public static void ChangeTargetRenderSettings( RenderSettingsData renderSettings, float shiftTime )
	{
		instance.IChangeTargetRenderSettings( renderSettings, shiftTime );
	}

	void IChangeTargetRenderSettings( RenderSettingsData renderSettings, float shiftTime )
	{
		_targetRenderSettings = renderSettings;

		StopAllCoroutines();
		StartCoroutine( GoToTargetRenderSettings( shiftTime ) );
	}

	void ApplyRenderSettings()
	{
		Color skyboxColor = Color.Lerp( _currentRenderSettings.daySettings.skyColor,
		                                _currentRenderSettings.nightSettings.skyColor,
		                                timeOfDay );

		if( _currentRenderSettings.skyMaterial != _curSkyboxMaterial)
		{
			_curSkyboxMaterial = _targetRenderSettings.skyMaterial;
			RenderSettings.skybox = _curSkyboxMaterial;
		}

		if( _curSkyboxMaterial )
		{
			RenderSettings.skybox.SetColor( _curSkyboxTintPropertyID, skyboxColor );
		}
		
		RenderSettings.fogColor = Color.Lerp( _currentRenderSettings.daySettings.fogColor,
		                                     _currentRenderSettings.nightSettings.fogColor,
		                                     timeOfDay );

		RenderSettings.fogDensity = Mathf.Lerp( _currentRenderSettings.daySettings.fogDensity,
		                                        _currentRenderSettings.nightSettings.fogDensity,
		                                        timeOfDay);

		if ( _dirLight )
		{
			_dirLight.color = Color.Lerp( _currentRenderSettings.daySettings.lightColor,
			                             _currentRenderSettings.nightSettings.lightColor,
			                             timeOfDay );

			_dirLight.intensity = Mathf.Lerp( _currentRenderSettings.daySettings.lightIntensity,
			                                  _currentRenderSettings.nightSettings.lightIntensity,
			                                  timeOfDay);
		}
		else
		{
			Debug.LogWarning( "Cannot change light values, no light is available." );
		}
	}

	void LerpRenderSettings( RenderSettingsData initRenderSettings, float delta )
	{
		RenderSettingsData renderSettings = new RenderSettingsData();
		renderSettings.daySettings = new TimeRenderSettings();
		renderSettings.nightSettings = new TimeRenderSettings();


		// Day Settings

		renderSettings.daySettings.skyColor = Color.Lerp( initRenderSettings.daySettings.skyColor, 
		                                                 _targetRenderSettings.daySettings.skyColor,
		                                                 delta );

		renderSettings.daySettings.fogColor = Color.Lerp( initRenderSettings.daySettings.fogColor, 
		                                                  _targetRenderSettings.daySettings.fogColor,
		                                                  delta );

		renderSettings.daySettings.fogDensity = Mathf.Lerp( initRenderSettings.daySettings.fogDensity,
		                                                    _targetRenderSettings.daySettings.fogDensity,
		                                                    delta );

		renderSettings.daySettings.lightColor = Color.Lerp( initRenderSettings.daySettings.lightColor, 
		                                                    _targetRenderSettings.daySettings.lightColor,
		                                                    delta );

		renderSettings.daySettings.lightIntensity = Mathf.Lerp( initRenderSettings.daySettings.lightIntensity,
		                                                        _targetRenderSettings.daySettings.lightIntensity,
		                                                        delta );

		// Night Settings

		renderSettings.nightSettings.skyColor = Color.Lerp( initRenderSettings.nightSettings.skyColor, 
		                                                    _targetRenderSettings.nightSettings.skyColor,
		                                                    delta );
		
		renderSettings.nightSettings.fogColor = Color.Lerp( initRenderSettings.nightSettings.fogColor, 
		                                                   _targetRenderSettings.nightSettings.fogColor,
		                                                   delta );
		
		renderSettings.nightSettings.fogDensity = Mathf.Lerp( initRenderSettings.nightSettings.fogDensity,
		                                                     _targetRenderSettings.nightSettings.fogDensity,
		                                                     delta );
		
		renderSettings.nightSettings.lightColor = Color.Lerp( initRenderSettings.nightSettings.lightColor, 
		                                                     _targetRenderSettings.nightSettings.lightColor,
		                                                     delta );
		
		renderSettings.nightSettings.lightIntensity = Mathf.Lerp( initRenderSettings.nightSettings.lightIntensity,
		                                                         _targetRenderSettings.nightSettings.lightIntensity,
		                                                         delta );

		_currentRenderSettings = renderSettings;
	}

	IEnumerator GoToTargetRenderSettings( float settingShiftTime )
	{
		RenderSettingsData initRenderSettings = _currentRenderSettings;

		float settingShiftTimer = 0f;
		while ( settingShiftTimer < settingShiftTime )
		{
			LerpRenderSettings( initRenderSettings, settingShiftTimer/settingShiftTime );

			settingShiftTimer += Time.deltaTime;
			yield return 0;
		}

		LerpRenderSettings( initRenderSettings, 1f );
		ApplyRenderSettings();
	}
}
