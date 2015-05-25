using UnityEngine;
using System.Collections;

[System.Serializable]
public class LightingSettings 
{
	public TimeLightingSettings daySettings = null;
	public TimeLightingSettings nightSettings = null;

	public LightingSettings()
	{

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
