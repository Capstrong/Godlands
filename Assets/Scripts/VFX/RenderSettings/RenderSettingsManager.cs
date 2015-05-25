using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RenderSettingsManager : SingletonBehaviour<RenderSettingsManager> 
{
	// Render Settings as we use them are the aesthetics of the scene:
	// This includes: 
	// - fog strength and color
	// - light strength and color
	// - skybox color and rotation

	[ReadOnly, SerializeField] 
	TimeLightingSettings _currentTimeLightingSettings = null; // This visualizes the current render settings
	public TimeLightingSettings currentTimeLightingSettings
	{
		get
		{
			if( _currentTimeLightingSettings == null )
			{
				_currentTimeLightingSettings = new TimeLightingSettings();
			}

			return _currentTimeLightingSettings;
		}
	}

	[ReadOnly, SerializeField]
	LightingSettings _currentLightingSettings = new LightingSettings(); // This visualizes the current day/night settings we're interpolating between

	// Daylight calculation
	[Tooltip( "The percent of the end of the day cycle that stays completely dark." )]
	[SerializeField] float _nightFraction = 0.1f;
	
	[ReadOnly("Daylight Intensity"), Tooltip( "0 is midnight. 1 is noon." ), SerializeField] 
	float _daylightIntensity = 1.0f;
	public static float daylightIntensity
	{
		get { return instance._daylightIntensity; }
		private set { instance._daylightIntensity = value; }
	}

	// Shader properties
	int _skyboxTintPropertyID = 0;
	int _skyboxRotationPropertyID = 0;
	int _fogGradientTexturePropertyID = 0;

	// Fog Gradient
	int _gradientTextureWidth = 128;
	int _gradientTextureHeight = 1;

	Texture2D _gradientTexture = null;
	public Texture2D gradientTexture
	{
		get
		{
			if( !_gradientTexture )
			{
				_gradientTexture = new Texture2D( _gradientTextureWidth, _gradientTextureHeight, TextureFormat.ARGB32, false, false );
				_gradientTexture.filterMode = FilterMode.Point;
			}

			return _gradientTexture;
		}
	}

	GlobalGradientFog _globalFog = null;
	public GlobalGradientFog globalFog
	{
		get
		{
			if( !_globalFog )
			{
				_globalFog = GameObject.FindObjectOfType<GlobalGradientFog>();
				if( !_globalFog )
				{
					Debug.LogWarning( "Global fog doesn't exist so render settings will be broken" );
				}
			}
			
			return _globalFog;
		}
	}

	// Lighting
	[SerializeField] Light _directionalLight = null;

	// Skybox rotation
	float _skyboxInitRotation = 0f;
	[SerializeField, Range(0f, 5f)] float _skyboxRotSpeed = 1f;

	void Awake()
	{
		_skyboxTintPropertyID = Shader.PropertyToID( "_Tint" );
		_skyboxRotationPropertyID = Shader.PropertyToID( "_Rotation" );
		_fogGradientTexturePropertyID = Shader.PropertyToID( "_FogGradientTex" );

		_gradientTexture = new Texture2D( _gradientTextureWidth, _gradientTextureHeight, TextureFormat.ARGB32, false, false );
		_gradientTexture.filterMode = FilterMode.Point;

		RenderSettings.skybox.SetFloat( _skyboxRotationPropertyID, _skyboxInitRotation );

		SetToNearestZone( Camera.main.GetComponent<Transform>().position );
	}

	void Update()
	{
		RenderSettings.skybox.SetFloat( _skyboxRotationPropertyID, Time.realtimeSinceStartup * _skyboxRotSpeed );
	
		float timeOfDay = DayCycleManager.currentTime / ( DayCycleManager.dayCycleLength * ( 1.0f - _nightFraction) );
		timeOfDay = Mathf.Clamp01( timeOfDay );
		
		daylightIntensity = Mathf.Cos( timeOfDay * 2.0f * Mathf.PI ) * -0.5f + 0.5f;

		if( currentTimeLightingSettings != null )
		{
			TimeLightingSettings.Lerp( _currentLightingSettings.nightSettings,
			                           _currentLightingSettings.daySettings,
			                           daylightIntensity,
			                           ref _currentTimeLightingSettings );

			if( globalFog.fogMaterial != null )
			{
				Color[] gradientColors = new Color[_gradientTextureWidth];
				for( int i = 0; i < _gradientTextureWidth; i++ )
				{
					gradientColors[i] = currentTimeLightingSettings.fogGradient.Evaluate( i/(float)(_gradientTextureWidth - 1) );
				}
				gradientTexture.SetPixels( gradientColors );
				gradientTexture.Apply();

				globalFog.fogMaterial.SetTexture( _fogGradientTexturePropertyID, gradientTexture );
			}
			
			RenderSettings.skybox.SetColor( _skyboxTintPropertyID, currentTimeLightingSettings.skyColor );

			_directionalLight.color = currentTimeLightingSettings.lightColor;
			_directionalLight.intensity = currentTimeLightingSettings.lightIntensity;
		}
	}

	public static void TransitionToNearestZone( Vector3 position, float time )
	{
		TransitionRenderSettings( GetNearestRenderSettings( position ), time );
	}

	public static void TransitionRenderSettings( LightingSettings newSettings, float shiftTime )
	{
		instance.StopAllCoroutines(); // TODO: Make this not a sledgehammer solution
		instance.StartCoroutine( instance.TransitionRenderSettingsRoutine( newSettings, shiftTime ) );
	}

	IEnumerator TransitionRenderSettingsRoutine( LightingSettings newSettings, float settingShiftTime )
	{
		LightingSettings oldRenderSettings = _currentLightingSettings;

		float settingShiftTimer = 0f;
		while ( settingShiftTimer < settingShiftTime )
		{
			LightingSettings.Lerp( oldRenderSettings,
			                       newSettings,
			                       settingShiftTimer/settingShiftTime,
			                       ref _currentLightingSettings );

			settingShiftTimer += Time.deltaTime;
			yield return 0;
		}

		_currentLightingSettings = newSettings;
	}

	public static void SetToNearestZone( Vector3 position )
	{
		SetRenderSettings( GetNearestRenderSettings( position ) );
	}

	public static void SetRenderSettings( LightingSettings lightSettings )
	{
		instance._currentLightingSettings = lightSettings;
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

			return nearestZone.lightingSettings;
		}

		Debug.LogError( "No render zones available. Creating one for temporary use." );
		return new LightingSettings();
	}

	void OnDisable()
	{
		DestroyImmediate( _gradientTexture );
	}

	void OnDestroy()
	{
		DestroyImmediate( _gradientTexture );
	}
}
