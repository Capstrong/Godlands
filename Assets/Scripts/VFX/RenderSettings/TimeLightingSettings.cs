using UnityEngine;
using System.Collections;

[System.Serializable]
public struct TimeLightingSettings 
{
	public Color skyColor;
	public Color lightColor;
	public float lightIntensity;
	public float fogDensity;
	public Gradient fogGradient;

	public TimeLightingSettings( Gradient gradient )
	{
		skyColor = Color.white;
		lightColor = Color.white;
		lightIntensity = 1f;
		fogDensity = 0.05f;
		fogGradient = gradient;
	}

	public static void Lerp( TimeLightingSettings a, TimeLightingSettings b, float t, ref TimeLightingSettings c )
	{
		c.skyColor = Color.Lerp( a.skyColor, b.skyColor, t );
		c.lightColor = Color.Lerp( a.lightColor, b.lightColor, t );
		c.lightIntensity = Mathf.Lerp( a.lightIntensity, b.lightIntensity, t );
		c.fogDensity = Mathf.Lerp( a.fogDensity, b.fogDensity, t );
		
		WadeUtils.Lerp( a.fogGradient, b.fogGradient, t, ref c.fogGradient );
	}
}