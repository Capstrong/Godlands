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

	[ReadOnly("Daylight Intensity"), Tooltip( "0 is midnight. 1 is noon." )]
	[SerializeField]
	float _daylightIntensity = 1.0f;
	public float daylightIntensity
	{
		get
		{
			return _daylightIntensity;
		}
		private set
		{
			_daylightIntensity = value;
		}
	}

	void Awake()
	{
		_curSkyboxMaterial = _currentRenderSettings.skyMaterial;
		_curSkyboxTintPropertyID = Shader.PropertyToID( "_Tint" );
	}

	void Update()
	{
		daylightIntensity = Mathf.Cos( DayCycleManager.instance.dayCycleTimer / DayCycleManager.instance.dayCycleLength * 2 * Mathf.PI ) * 0.5f + 0.5f;

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
		                                daylightIntensity );

		if ( _currentRenderSettings.skyMaterial != _curSkyboxMaterial )
		{
			_curSkyboxMaterial = _targetRenderSettings.skyMaterial;
			RenderSettings.skybox = _curSkyboxMaterial;
		}

		if ( _curSkyboxMaterial )
		{
			RenderSettings.skybox.SetColor( _curSkyboxTintPropertyID, skyboxColor );
		}

		RenderSettings.fogColor = Color.Lerp( _currentRenderSettings.daySettings.fogColor,
		                                      _currentRenderSettings.nightSettings.fogColor,
		                                      daylightIntensity );

		RenderSettings.fogDensity = Mathf.Lerp( _currentRenderSettings.daySettings.fogDensity,
		                                        _currentRenderSettings.nightSettings.fogDensity,
		                                        daylightIntensity );

		if ( _dirLight )
		{
			_dirLight.color = Color.Lerp( _currentRenderSettings.daySettings.lightColor,
			                              _currentRenderSettings.nightSettings.lightColor,
			                              daylightIntensity );

			_dirLight.intensity = Mathf.Lerp( _currentRenderSettings.daySettings.lightIntensity,
			                                  _currentRenderSettings.nightSettings.lightIntensity,
			                                  daylightIntensity );
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
