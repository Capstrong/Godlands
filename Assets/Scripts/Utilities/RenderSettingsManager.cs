using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct TimeRenderSettings
{
	public Color skyColor;
	public Color lightColor;
	public float lightIntensity;
	public Color fogColor;
	public float fogDensity;

	public static void Lerp(TimeRenderSettings fromSettings, TimeRenderSettings toSettings, float t, out TimeRenderSettings outSettings)
	{
		outSettings.skyColor =       Color.Lerp( fromSettings.skyColor,       toSettings.skyColor,       t );
		outSettings.lightColor =     Color.Lerp( fromSettings.lightColor,     toSettings.lightColor,     t );
		outSettings.lightIntensity = Mathf.Lerp( fromSettings.lightIntensity, toSettings.lightIntensity, t );
		outSettings.fogColor =       Color.Lerp( fromSettings.fogColor,       toSettings.fogColor,       t );
		outSettings.fogDensity =     Mathf.Lerp( fromSettings.fogDensity,     toSettings.fogDensity,     t );
	}
}

[Serializable]
public struct RenderSettingsData
{
	public TimeRenderSettings daySettings;
	public TimeRenderSettings nightSettings;

	public Material skyMaterial;

	public static void Lerp(RenderSettingsData fromSettings, RenderSettingsData toSettings, float t, out RenderSettingsData outSettings )
	{
		TimeRenderSettings.Lerp( fromSettings.daySettings, toSettings.daySettings, t, out outSettings.daySettings );
		TimeRenderSettings.Lerp( fromSettings.nightSettings, toSettings.nightSettings, t, out outSettings.nightSettings );
		outSettings.skyMaterial = ( t < 0.5f ) ? fromSettings.skyMaterial : toSettings.skyMaterial;
	}
}

[ExecuteInEditMode]
public class RenderSettingsManager : SingletonBehaviour<RenderSettingsManager> 
{
	[SerializeField] RenderSettingsData _currentRenderSettings = new RenderSettingsData();
	RenderSettingsData _currentRenderSettingsProperty
	{
		get { return _currentRenderSettings; }

		set
		{
			_currentRenderSettings = value;
			RenderSettings.skybox = _currentRenderSettings.skyMaterial;
		}
	}

	[SerializeField] Light _dirLight = null;
	int _curSkyboxTintPropertyID = 0;

	[Tooltip( "The percent of the end of the day cycle that stays completely dark." )]
	[SerializeField] float _nightFraction = 0.1f;

	[ReadOnly]
	[SerializeField] TimeRenderSettings _currentTimeRenderSettings = new TimeRenderSettings();

	[ReadOnly("Daylight Intensity"), Tooltip( "0 is midnight. 1 is noon." )]
	[SerializeField] float _daylightIntensity = 1.0f;
	public float daylightIntensity
	{
		get { return _daylightIntensity; }
		private set { _daylightIntensity = value; }
	}

	void Awake()
	{
		_curSkyboxTintPropertyID = Shader.PropertyToID( "_Tint" );
	}

	void Update()
	{
		float timeOfDay = timeOfDay = DayCycleManager.currentTime / ( DayCycleManager.dayCycleLength * ( 1.0f - _nightFraction) );
		timeOfDay = Mathf.Clamp01( timeOfDay );
		daylightIntensity = Mathf.Cos( timeOfDay * 2.0f * Mathf.PI ) * -0.5f + 0.5f;

		TimeRenderSettings.Lerp( _currentRenderSettings.nightSettings,
		                         _currentRenderSettings.daySettings,
		                         daylightIntensity,
		                         out _currentTimeRenderSettings );
		UpdateRenderSettings();
	}

	public static void TransitionRenderSettings( RenderSettingsData newRenderSettings, float shiftTime )
	{
		instance.StopAllCoroutines(); // TODO: Make this not a sledgehammer solution
		instance.StartCoroutine( instance.TransitionRenderSettingsRoutine( newRenderSettings, shiftTime ) );
	}

	IEnumerator TransitionRenderSettingsRoutine( RenderSettingsData newRenderSettings, float settingShiftTime )
	{
		RenderSettingsData oldRenderSettings = _currentRenderSettingsProperty;

		float settingShiftTimer = 0f;
		while ( settingShiftTimer < settingShiftTime )
		{
			RenderSettingsData.Lerp( oldRenderSettings, newRenderSettings, settingShiftTimer / settingShiftTime, out _currentRenderSettings );
			UpdateRenderSettings();

			settingShiftTimer += Time.deltaTime;
			yield return 0;
		}

		_currentRenderSettingsProperty = newRenderSettings;
	}

	void UpdateRenderSettings()
	{
		RenderSettings.skybox.SetColor( _curSkyboxTintPropertyID, _currentTimeRenderSettings.skyColor );
		RenderSettings.fogColor = _currentTimeRenderSettings.fogColor;
		RenderSettings.fogDensity = _currentTimeRenderSettings.fogDensity;
		_dirLight.color = _currentTimeRenderSettings.lightColor;
		_dirLight.intensity = _currentTimeRenderSettings.lightIntensity;
	}
}
