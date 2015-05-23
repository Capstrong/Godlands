using UnityEngine;
using System.Collections;

[System.Serializable]
public class LightingSettings 
{
	public TimeLightingSettings daySettings = new TimeLightingSettings();
	public TimeLightingSettings nightSettings = new TimeLightingSettings();

	public LightingSettings()
	{
		daySettings = new TimeLightingSettings( new Gradient() );
		nightSettings = new TimeLightingSettings( new Gradient() );
	}

	public LightingSettings( TimeLightingSettings day, TimeLightingSettings night )
	{
		daySettings = day;
		nightSettings = night;
	}

	public static void Lerp( LightingSettings a, LightingSettings b, float t, ref LightingSettings c )
	{
		TimeLightingSettings.Lerp( a.daySettings, b.daySettings, t, ref c.daySettings );
		TimeLightingSettings.Lerp( a.nightSettings, b.nightSettings, t, ref c.nightSettings );
	}
}
