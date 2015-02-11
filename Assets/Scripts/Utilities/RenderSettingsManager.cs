using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class LightSettings
{
	public Color color;
	public float intensity;
}

[Serializable]
public class FogSettings
{
	public Color color;
	public float density;
}

[Serializable]
public class RenderSettingsData
{
	public Color dayColor;
	public Color nightColor;

	public LightSettings lightSettings;
	public FogSettings fogSettings;
}

public class RenderSettingsManager : SingletonBehaviour<RenderSettingsManager> 
{
	[SerializeField] RenderSettingsData _currentRenderSettings = null;
	RenderSettingsData _targetRenderSettings = new RenderSettingsData();

	[SerializeField] float _settingShiftTime = 15f;
	float _settingShiftTimer = 0f;

	Light _dirLight = null;

	[Range(0, 1)]
	public float timeOfDay; // This should not go in here. Eventually it will be with the Day Cycle

	void Awake()
	{
		_dirLight = GameObject.FindObjectOfType<Light>();
	}

	void Update()
	{
		ApplyRenderSettings();
	}

	public static void ChangeTargetRenderSettings( RenderSettingsData renderSettings )
	{
		instance.IChangeTargetRenderSettings( renderSettings );
	}

	void IChangeTargetRenderSettings( RenderSettingsData renderSettings )
	{
		_targetRenderSettings = renderSettings;

		StopAllCoroutines();
		StartCoroutine( GoToTargetRenderSettings() );
	}

	void ApplyRenderSettings()
	{
		Color skyboxColor = Color.Lerp( _currentRenderSettings.dayColor,
		                                _currentRenderSettings.nightColor,
		                                timeOfDay );
		RenderSettings.skybox.SetColor( "_Tint", skyboxColor );

		RenderSettings.fogColor = _currentRenderSettings.fogSettings.color;
		RenderSettings.fogDensity = _currentRenderSettings.fogSettings.density;

		if ( _dirLight )
		{
			_dirLight.color = _currentRenderSettings.lightSettings.color;
			_dirLight.intensity = _currentRenderSettings.lightSettings.intensity;
		}
		else
		{
			Debug.LogWarning( "Cannot change light values, no light is available." );
		}
	}

	void LerpRenderSettings( RenderSettingsData initRenderSettings, float delta )
	{
		RenderSettingsData renderSettings = new RenderSettingsData();
		renderSettings.dayColor = Color.Lerp( initRenderSettings.dayColor, 
		                                      _targetRenderSettings.dayColor,
		                                      delta );

		renderSettings.nightColor = Color.Lerp( initRenderSettings.nightColor, 
		                                     	_targetRenderSettings.nightColor,
		                                     	delta );

		LightSettings lightSettings = new LightSettings();
		lightSettings.color = Color.Lerp( initRenderSettings.lightSettings.color,
		                                  _targetRenderSettings.lightSettings.color,
		                                  delta );

		lightSettings.intensity = Mathf.Lerp( initRenderSettings.lightSettings.intensity,
		                                      _targetRenderSettings.lightSettings.intensity,
		                                      delta );

		FogSettings fogSettings = new FogSettings();
		fogSettings.color = Color.Lerp(	initRenderSettings.fogSettings.color,
		                                _targetRenderSettings.fogSettings.color,
		                                delta );

		fogSettings.density = Mathf.Lerp( initRenderSettings.fogSettings.density,
		                                  _targetRenderSettings.fogSettings.density,
		                                  delta );

		renderSettings.lightSettings = lightSettings;
		renderSettings.fogSettings = fogSettings;

		_currentRenderSettings = renderSettings;
	}

	IEnumerator GoToTargetRenderSettings()
	{
		RenderSettingsData initRenderSettings = _currentRenderSettings;

		_settingShiftTimer = 0f;
		while ( _settingShiftTimer < _settingShiftTime )
		{
			LerpRenderSettings( initRenderSettings, _settingShiftTimer/_settingShiftTime );

			_settingShiftTimer += Time.deltaTime;
			yield return 0;
		}

		LerpRenderSettings( initRenderSettings, 1f );
		ApplyRenderSettings();
	}
}
