using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct TimeRenderSettings
{
	public Color skyColor;
	public Color lightColor;
	public float lightIntensity;
	public Texture2D fogGradientTex;
	public float fogDensity;

	public static void Lerp(TimeRenderSettings fromSettings, TimeRenderSettings toSettings, float t, out TimeRenderSettings outSettings)
	{
		outSettings.skyColor =       Color.Lerp( fromSettings.skyColor,       toSettings.skyColor,       t );
		outSettings.lightColor =     Color.Lerp( fromSettings.lightColor,     toSettings.lightColor,     t );
		outSettings.lightIntensity = Mathf.Lerp( fromSettings.lightIntensity, toSettings.lightIntensity, t );
		outSettings.fogDensity =     Mathf.Lerp( fromSettings.fogDensity,     toSettings.fogDensity,     t );

		outSettings.fogGradientTex = fromSettings.fogGradientTex;

		// cannot lerp because this is a texture
		// outSettings.fogGradientTex = outSettings.fogGradientTex;
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

	float _skyboxInitRotation = 0f;
	[SerializeField, Range(0f, 5f)] float _skyboxRotSpeed = 1f;

	[Tooltip( "The percent of the end of the day cycle that stays completely dark." )]
	[SerializeField] float _nightFraction = 0.1f;

	[ReadOnly]
	[SerializeField] TimeRenderSettings _currentTimeRenderSettings = new TimeRenderSettings();
	public static TimeRenderSettings currentTimeRenderSettings
	{
		get
		{
			return instance._currentTimeRenderSettings;
		}
	}

	UnityStandardAssets.ImageEffects.GlobalFog _globalFog = null;
	public UnityStandardAssets.ImageEffects.GlobalFog globalFog
	{
		get
		{
			if( !_globalFog )
			{
				_globalFog = GameObject.FindObjectOfType<UnityStandardAssets.ImageEffects.GlobalFog>();
			}

			return _globalFog;
		}
	}

	[ReadOnly("Daylight Intensity"), Tooltip( "0 is midnight. 1 is noon." )]
	[SerializeField] float _daylightIntensity = 1.0f;
	public static float daylightIntensity
	{
		get { return instance._daylightIntensity; }
		private set { instance._daylightIntensity = value; }
	}

	void Awake()
	{
		_curSkyboxTintPropertyID = Shader.PropertyToID( "_Tint" );
		RenderSettings.skybox.SetFloat( "_Rotation", _skyboxInitRotation );
	}

	void Update()
	{
		RenderSettings.skybox.SetFloat( "_Rotation", Time.realtimeSinceStartup * _skyboxRotSpeed );

		float timeOfDay = DayCycleManager.currentTime / ( DayCycleManager.dayCycleLength * ( 1.0f - _nightFraction) );
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
		if( _currentRenderSettings.daySettings.fogGradientTex )
		{
			globalFog.fogMaterial.SetTexture("_FromDayTex", _currentRenderSettingsProperty.daySettings.fogGradientTex );
			globalFog.fogMaterial.SetTexture("_FromNightTex", _currentRenderSettingsProperty.nightSettings.fogGradientTex );
		}
		else
		{
			globalFog.fogMaterial.SetTexture( "_ToDayTex", newRenderSettings.daySettings.fogGradientTex );
			globalFog.fogMaterial.SetTexture( "_ToNightTex", newRenderSettings.daySettings.fogGradientTex );
		}

		globalFog.fogMaterial.SetTexture( "_ToDayTex", newRenderSettings.daySettings.fogGradientTex );
		globalFog.fogMaterial.SetTexture( "_ToNightTex", newRenderSettings.daySettings.fogGradientTex );

		RenderSettingsData oldRenderSettings = _currentRenderSettingsProperty;

		float settingShiftTimer = 0f;
		while ( settingShiftTimer < settingShiftTime )
		{
			RenderSettingsData.Lerp( oldRenderSettings, 
			                         newRenderSettings, 
			                         settingShiftTimer / settingShiftTime, 
			                         out _currentRenderSettings );
			UpdateRenderSettings();

			globalFog.fogMaterial.SetFloat( "_TransitionTime", settingShiftTimer/settingShiftTime );

			settingShiftTimer += Time.deltaTime;
			yield return 0;
		}

		_currentRenderSettingsProperty = newRenderSettings;
	}

	void UpdateRenderSettings()
	{
		RenderSettings.skybox.SetColor( _curSkyboxTintPropertyID, _currentTimeRenderSettings.skyColor );

		globalFog.fogMaterial.SetFloat( "_DaylightIntensity", daylightIntensity );
		RenderSettings.fogDensity = _currentTimeRenderSettings.fogDensity;

		_dirLight.color = _currentTimeRenderSettings.lightColor;
		_dirLight.intensity = _currentTimeRenderSettings.lightIntensity;
	}
}
