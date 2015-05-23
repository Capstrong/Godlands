using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RenderSettingsManager : SingletonBehaviour<RenderSettingsManager> 
{
	LightingSettings _currentLightingSettings = new LightingSettings();

	[ReadOnly, SerializeField] 
	TimeLightingSettings _currentTimeRenderSettings = new TimeLightingSettings();

	Texture2D _gradientTexture = null;
	public Texture2D gradientTexture
	{
		get
		{
			if( !_gradientTexture )
			{
				_gradientTexture = new Texture2D( 128, 1, TextureFormat.ARGB32, false );
			}

			return _gradientTexture;
		}
	}

	[SerializeField] Light _directionalLight = null;
	int _skyboxTintPropertyID = 0;

	float _skyboxInitRotation = 0f;
	[SerializeField, Range(0f, 5f)] float _skyboxRotSpeed = 1f;

	[Tooltip( "The percent of the end of the day cycle that stays completely dark." )]
	[SerializeField] float _nightFraction = 0.1f;

	UnityStandardAssets.ImageEffects.GlobalFog _globalFog = null;
	public UnityStandardAssets.ImageEffects.GlobalFog globalFog
	{
		get
		{
			if( !_globalFog )
			{
				_globalFog = GameObject.FindObjectOfType<UnityStandardAssets.ImageEffects.GlobalFog>();
				if( !_globalFog )
				{
					Debug.LogWarning( "Global fog doesn't exist so render settings will be broken" );
				}
			}

			return _globalFog;
		}
	}

	[ReadOnly("Daylight Intensity"), Tooltip( "0 is midnight. 1 is noon." ), SerializeField] 
	float _daylightIntensity = 1.0f;
	public static float daylightIntensity
	{
		get { return instance._daylightIntensity; }
		private set { instance._daylightIntensity = value; }
	}

	void Awake()
	{
		_skyboxTintPropertyID = Shader.PropertyToID( "_Tint" );
		RenderSettings.skybox.SetFloat( "_Rotation", _skyboxInitRotation );

		_gradientTexture = null;
		_gradientTexture = new Texture2D( 128, 1, TextureFormat.ARGB32, false );

		UpdateTimeOfDay();
		UpdateFogGradientTexture();
	}

	void Update()
	{
		RenderSettings.skybox.SetFloat( "_Rotation", Time.realtimeSinceStartup * _skyboxRotSpeed );
	
		UpdateRenderSettings();
	}

	void UpdateTimeOfDay()
	{
		float timeOfDay = DayCycleManager.currentTime / ( DayCycleManager.dayCycleLength * ( 1.0f - _nightFraction) );
		timeOfDay = Mathf.Clamp01( timeOfDay );
		
		daylightIntensity = Mathf.Cos( timeOfDay * 2.0f * Mathf.PI ) * -0.5f + 0.5f;
		
		TimeLightingSettings.Lerp( _currentLightingSettings.nightSettings,
		                           _currentLightingSettings.daySettings,
		                           daylightIntensity,
		                           ref _currentTimeRenderSettings );
	}

	void UpdateFogGradientTexture()
	{
		Color[] gradientColors = new Color[128];
		for( int i = 0; i < 128; i++ )
		{
			gradientColors[i] = _currentTimeRenderSettings.fogGradient.Evaluate( i/128f );
		}
		gradientTexture.SetPixels( gradientColors );
		gradientTexture.Apply();
	}

	void UpdateRenderSettings()
	{
		UpdateTimeOfDay();

		if( globalFog.fogMaterial != null )
		{
			UpdateFogGradientTexture();
			globalFog.fogMaterial.SetTexture( "_FogGradientTex", gradientTexture );
		}
		
		RenderSettings.skybox.SetColor( _skyboxTintPropertyID, _currentTimeRenderSettings.skyColor );
		RenderSettings.fogDensity = _currentTimeRenderSettings.fogDensity;
		
		_directionalLight.color = _currentTimeRenderSettings.lightColor;
		_directionalLight.intensity = _currentTimeRenderSettings.lightIntensity;
	}

	public static void TransitionRenderSettings( LightingSettings newRenderSettings, float shiftTime )
	{
		instance.StopAllCoroutines(); // TODO: Make this not a sledgehammer solution
		instance.StartCoroutine( instance.TransitionRenderSettingsRoutine( newRenderSettings, shiftTime ) );
	}

	IEnumerator TransitionRenderSettingsRoutine( LightingSettings newRenderSettings, float settingShiftTime )
	{
		LightingSettings oldRenderSettings = _currentLightingSettings;

		float settingShiftTimer = 0f;
		while ( settingShiftTimer < settingShiftTime )
		{
			TimeLightingSettings.Lerp( oldRenderSettings.daySettings, 
			                           newRenderSettings.daySettings,
			                           settingShiftTimer/settingShiftTime,
			                           ref _currentLightingSettings.daySettings );

			TimeLightingSettings.Lerp( oldRenderSettings.nightSettings,
			                           newRenderSettings.nightSettings,
			                           settingShiftTimer/settingShiftTime,
			                           ref _currentLightingSettings.nightSettings );

			UpdateRenderSettings();

			settingShiftTimer += Time.deltaTime;
			yield return 0;
		}

		_currentLightingSettings = newRenderSettings;
	}

	public static void SetRenderSettings( LightingSettings lightSettings )
	{
		instance._currentLightingSettings = lightSettings;
		instance.UpdateRenderSettings();
	}

	public static void SetToNearestZone( Vector3 position )
	{
		SetRenderSettings( GetNearestRenderSettings( position ) );
	}

	public static void TransitionToNearestZone( Vector3 position, float time )
	{
		TransitionRenderSettings( GetNearestRenderSettings( position ), time );
	}

	public static LightingSettings GetNearestRenderSettings( Vector3 position )
	{
		RenderSettingsZone nearestZone = null;

		RenderSettingsZone[] renderSettingsZones = GameObject.FindObjectsOfType<RenderSettingsZone>();
		if( renderSettingsZones.Length > 0 )
		{
			float nearestZoneDist = Mathf.Infinity;
			foreach( RenderSettingsZone iter in renderSettingsZones )
			{
				float zoneDist = Vector3.Distance( iter.GetComponent<Transform>().position, position );
				if( zoneDist < nearestZoneDist )
				{
					nearestZoneDist = zoneDist;
					nearestZone = iter;
				}
			}

			return nearestZone.GetLightingSettings();
		}

		Debug.LogError( "No render zones available. Creating one for temporary use." );
		return new LightingSettings();
	}
}
