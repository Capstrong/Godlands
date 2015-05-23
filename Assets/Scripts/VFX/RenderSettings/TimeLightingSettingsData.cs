using UnityEngine;
using System.Collections;

public class TimeLightingSettingsData : ScriptableObject 
{
	public Color skyColor = Color.white;
	public Color lightColor = Color.white;
	public float lightIntensity = 1f;
	public float fogDensity = 0.05f;
	public Gradient fogGradient = new Gradient();

	public static void Lerp( TimeLightingSettingsData a, TimeLightingSettingsData b, float t, ref TimeLightingSettingsData c )
	{
		c.skyColor = Color.Lerp( a.skyColor, b.skyColor, t );
		c.lightColor = Color.Lerp( a.lightColor, b.lightColor, t );
		c.lightIntensity = Mathf.Lerp( a.lightIntensity, b.lightIntensity, t );
		c.fogDensity = Mathf.Lerp( a.fogDensity, b.fogDensity, t );
		
		WadeUtils.Lerp( a.fogGradient, b.fogGradient, t, ref c.fogGradient );
	}

	public TimeLightingSettings GetTimeLightingSettings()
	{
		TimeLightingSettings t = new TimeLightingSettings();
		t.skyColor = skyColor;
		t.lightColor = lightColor;
		t.lightIntensity = lightIntensity;
		t.fogDensity = fogDensity;
		t.fogGradient = fogGradient;

		return t;
	}
}