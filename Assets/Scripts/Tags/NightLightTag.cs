﻿using UnityEngine;
using System.Collections;

public class NightLightTag : MonoBehaviour
{
	[Range( 0.0f, 1.0f )]
	[SerializeField] float _timeOn = 0.6f;

	private Light _light;

	void Awake()
	{
		_light = GetComponent<Light>();
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
		float timeToOn = DayCycleManager.dayCycleLength * _timeOn - DayCycleManager.currentTime;
		Invoke( "EnableLight", timeToOn );
	}

	void EnableLight()
	{
		_light.enabled = true;
	}
}
