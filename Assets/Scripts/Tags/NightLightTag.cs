using UnityEngine;
using System.Collections;

public class NightLightTag : MonoBehaviour
{
	[Range( 0.0f, 1.0f )]
	[SerializeField] float _timeOn = 0.6f;
	[SerializeField] float _fadeTime = 1.0f;

	private Light _light;
	private float _lightIntensity;

	void Awake()
	{
		_light = GetComponent<Light>();
		_lightIntensity = _light.intensity;
	}

	void Start()
	{
		DayCycleManager.RegisterEndOfDayCallback( ResetLight );
		ResetLight();
	}

	void ResetLight()
	{
		_light.enabled = false;

		// calculate time until the light
		// needs to turn on again.
		float timeOnSeconds = DayCycleManager.dayCycleLength * _timeOn;
		if ( DayCycleManager.currentTime < timeOnSeconds )
		{
			float timeToOn = timeOnSeconds - DayCycleManager.currentTime;
			Invoke( "EnableLight", timeToOn );
		}
		else
		{
			EnableLight();
		}
	}

	void EnableLight()
	{
		_light.enabled = true;
		_light.intensity = 0.0f;

		StartCoroutine( FadeLight() );
	}

	IEnumerator FadeLight()
	{
		while ( _light.intensity < _lightIntensity )
		{
			_light.intensity += _lightIntensity * Time.deltaTime / _fadeTime;
			yield return null;
		}
	}
}
