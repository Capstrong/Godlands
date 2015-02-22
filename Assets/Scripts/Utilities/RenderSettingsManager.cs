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

	public static TimeRenderSettings Lerp(TimeRenderSettings fromSettings, TimeRenderSettings toSettings, float t)
	{
		TimeRenderSettings returnSettings = new TimeRenderSettings();

		returnSettings.skyColor = Color.Lerp(       fromSettings.skyColor,       toSettings.skyColor, t );
		returnSettings.lightColor = Color.Lerp(     fromSettings.lightColor,     toSettings.lightColor, t );
		returnSettings.lightIntensity = Mathf.Lerp( fromSettings.lightIntensity, toSettings.lightIntensity, t );
		returnSettings.fogColor = Color.Lerp(       fromSettings.fogColor,       toSettings.fogColor, t );
		returnSettings.fogDensity = Mathf.Lerp(     fromSettings.fogDensity,     toSettings.fogDensity, t );

		return returnSettings;
	}
}

[Serializable]
public struct RenderSettingsData
{
	public TimeRenderSettings daySettings;
	public TimeRenderSettings nightSettings;

	public Material skyMaterial;

	public static RenderSettingsData Lerp(RenderSettingsData fromSettings, RenderSettingsData toSettings, float t)
	{
		RenderSettingsData returnSettings = new RenderSettingsData();

		returnSettings.daySettings = TimeRenderSettings.Lerp( fromSettings.daySettings, toSettings.daySettings, t );
		returnSettings.nightSettings = TimeRenderSettings.Lerp( fromSettings.nightSettings, toSettings.nightSettings, t );
		returnSettings.skyMaterial = ( t < 0.5f ) ? fromSettings.skyMaterial : toSettings.skyMaterial;

		return returnSettings;
	}
}

[ExecuteInEditMode]
public class RenderSettingsManager : SingletonBehaviour<RenderSettingsManager> 
{
	[SerializeField] RenderSettingsData _currentRenderSettings = new RenderSettingsData();
	[SerializeField] TimeRenderSettings _currentTimeRenderSettings = new TimeRenderSettings();

	[SerializeField] Light _dirLight = null;
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
		_curSkyboxTintPropertyID = Shader.PropertyToID( "_Tint" );
	}

	void Update()
	{
		daylightIntensity = Mathf.Cos( DayCycleManager.dayCycleTimer / DayCycleManager.dayCycleLength * 2 * Mathf.PI ) * 0.5f + 0.5f;

		_currentTimeRenderSettings = TimeRenderSettings.Lerp( _currentRenderSettings.nightSettings, _currentRenderSettings.daySettings, daylightIntensity );

		ApplyTimeRenderSettings();
	}

	public static void TransitionRenderSettings( RenderSettingsData newRenderSettings, float shiftTime )
	{
		instance._TransitionRenderSettings( newRenderSettings, shiftTime );
	}

	void _TransitionRenderSettings( RenderSettingsData newRenderSettings, float shiftTime )
	{
		StopAllCoroutines(); // TODO: Make this not a sledgehammer solution
		StartCoroutine( TransitionRenderSettingsRoutine( newRenderSettings, shiftTime ) );
	}

	void ApplyTimeRenderSettings()
	{
		RenderSettings.skybox = _currentRenderSettings.skyMaterial;
		RenderSettings.skybox.SetColor( _curSkyboxTintPropertyID, _currentTimeRenderSettings.skyColor );
		RenderSettings.fogColor = _currentTimeRenderSettings.fogColor;
		RenderSettings.fogDensity = _currentTimeRenderSettings.fogDensity;

		if ( _dirLight )
		{
			_dirLight.color = _currentTimeRenderSettings.lightColor;
			_dirLight.intensity = _currentTimeRenderSettings.lightIntensity;
		}
		else
		{
			Debug.LogWarning( "Cannot change light values, no light is available." );
		}
	}

	IEnumerator TransitionRenderSettingsRoutine( RenderSettingsData newRenderSettings, float settingShiftTime )
	{
		RenderSettingsData oldRenderSettings = _currentRenderSettings;

		float settingShiftTimer = 0f;
		while ( settingShiftTimer < settingShiftTime )
		{
			_currentRenderSettings = RenderSettingsData.Lerp( oldRenderSettings, newRenderSettings, settingShiftTimer / settingShiftTime );

			settingShiftTimer += Time.deltaTime;
			yield return 0;
		}

		_currentRenderSettings = newRenderSettings;
	}
}
