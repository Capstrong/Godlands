using UnityEngine;
using System.Collections;

[System.Serializable]
public class TimeLightingSettings
{
	public Color skyColor = Color.white;
	public Color lightColor = Color.white;
	public float lightIntensity = 1f;
	public Gradient fogGradient = new Gradient();
	
	public TimeLightingSettings()
	{
		skyColor = Color.white;
		lightColor = Color.white;
		lightIntensity = 1f;
		fogGradient = new Gradient();
	}
	
	public static void Lerp( TimeLightingSettings a, TimeLightingSettings b, float t, ref TimeLightingSettings c )
	{
		c.skyColor = Color.Lerp( a.skyColor, b.skyColor, t );
		c.lightColor = Color.Lerp( a.lightColor, b.lightColor, t );
		c.lightIntensity = Mathf.Lerp( a.lightIntensity, b.lightIntensity, t );
		WadeUtils.Lerp( a.fogGradient, b.fogGradient, t, ref c.fogGradient );
	}
	
	public void CopySettingsInto( ref TimeLightingSettings destination )
	{
		destination.skyColor = skyColor;
		destination.lightColor = lightColor;
		destination.lightIntensity = lightIntensity;
		WadeUtils.CopyValue( fogGradient, ref destination.fogGradient );
	}
}