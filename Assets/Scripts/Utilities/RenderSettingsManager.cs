using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class TimeRenderSettings
{
	public Color color = Color.white;

	public Color lightColor = Color.white;
	public float lightIntensity = 0.5f;

	public Color fogColor = Color.white;
	public float fogDensity = 0.05f;
}

[Serializable]
public class RenderSettingsData
{
	public TimeRenderSettings daySettings;
	public TimeRenderSettings nightSettings;
}

[ExecuteInEditMode]
public class RenderSettingsManager : SingletonBehaviour<RenderSettingsManager> 
{
	[SerializeField] RenderSettingsData _currentRenderSettings = null;
	RenderSettingsData _targetRenderSettings = new RenderSettingsData();

	[SerializeField] float _settingShiftTime = 15f;
	float _settingShiftTimer = 0f;

	Light _dirLight = null;

	[SerializeField] float dayCycleTime = 200;
	float dayCycleTimer = 0f;

	public float timeOfDay; // This should not go in here. Eventually it will be with the Day Cycle

	void Awake()
	{
		_dirLight = GameObject.FindObjectOfType<Light>();
	}

	void Update()
	{
		dayCycleTimer += Time.deltaTime;

		timeOfDay = (-Mathf.Cos(dayCycleTimer/dayCycleTime * Mathf.Rad2Deg) * 0.5f) + 0.5f;

		if(dayCycleTimer > dayCycleTime)
		{
			dayCycleTimer -= dayCycleTime;
		}

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
		Color skyboxColor = Color.Lerp( _currentRenderSettings.daySettings.color,
		                                _currentRenderSettings.nightSettings.color,
		                                timeOfDay );
		RenderSettings.skybox.SetColor( "_Tint", skyboxColor );

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

		renderSettings.daySettings.color = Color.Lerp( initRenderSettings.daySettings.color, 
		                                      	       _targetRenderSettings.daySettings.color,
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

		renderSettings.nightSettings.color = Color.Lerp( initRenderSettings.nightSettings.color, 
		                                                _targetRenderSettings.nightSettings.color,
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
