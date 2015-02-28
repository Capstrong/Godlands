﻿using UnityEngine;
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

	public static TimeRenderSettings Lerp(TimeRenderSettings fromSettings, TimeRenderSettings toSettings, float t)
	{
		TimeRenderSettings returnSettings = new TimeRenderSettings();

		returnSettings.skyColor =       Color.Lerp( fromSettings.skyColor,       toSettings.skyColor,       t );
		returnSettings.lightColor =     Color.Lerp( fromSettings.lightColor,     toSettings.lightColor,     t );
		returnSettings.lightIntensity = Mathf.Lerp( fromSettings.lightIntensity, toSettings.lightIntensity, t );
		returnSettings.fogColor =       Color.Lerp( fromSettings.fogColor,       toSettings.fogColor,       t );
		returnSettings.fogDensity =     Mathf.Lerp( fromSettings.fogDensity,     toSettings.fogDensity,     t );

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
	TimeRenderSettings _currentTimeRenderSettingsProperty
	{
		get { return _currentTimeRenderSettings; }

		set
		{
			_currentTimeRenderSettings = value;

			RenderSettings.skybox.SetColor( _curSkyboxTintPropertyID, _currentTimeRenderSettings.skyColor );
			RenderSettings.fogColor = _currentTimeRenderSettings.fogColor;
			RenderSettings.fogDensity = _currentTimeRenderSettings.fogDensity;
			_dirLight.color = _currentTimeRenderSettings.lightColor;
			_dirLight.intensity = _currentTimeRenderSettings.lightIntensity;
		}
	}

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
		float timeOfDay = DayCycleManager.dayCycleTimer / DayCycleManager.dayCycleLength;
		if ( timeOfDay < ( 1.0f - _nightFraction ) )
		{
			daylightIntensity =
			    Mathf.Cos( timeOfDay * 2.0f * Mathf.PI / ( 1.0f - _nightFraction ) ) * -0.5f + 0.5f;
		}
		else
		{
			daylightIntensity = 0.0f;
		}

		_currentTimeRenderSettingsProperty = TimeRenderSettings.Lerp( _currentRenderSettings.nightSettings, _currentRenderSettings.daySettings, daylightIntensity );
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
			_currentRenderSettingsProperty = RenderSettingsData.Lerp( oldRenderSettings, newRenderSettings, settingShiftTimer / settingShiftTime );

			settingShiftTimer += Time.deltaTime;
			yield return 0;
		}

		_currentRenderSettingsProperty = newRenderSettings;
	}
}
