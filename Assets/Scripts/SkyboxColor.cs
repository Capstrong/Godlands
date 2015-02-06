using UnityEngine;
using System.Collections;

public class SkyboxColor : MonoBehaviour 
{
	[SerializeField] Color startColor;
	[SerializeField] Color endColor;

	[SerializeField] bool useHSV = true;

	[SerializeField] Light light;
	[SerializeField] float lightBrightnessOffset = 1.2f;
	[SerializeField] float lightSaturationOffset = -0.3f;

	[SerializeField] float fogDensity = 0.008f;

	[SerializeField] float shiftTime = 30f;
	float shiftTimer = 0f;
	

	void Update()
	{
		if(shiftTimer < shiftTime)
		{
			Color lerpColor = new Color();

			if(!useHSV)
			{
				lerpColor = Color.Lerp( startColor, endColor, shiftTimer/shiftTime );
			}
			else
			{
				lerpColor = WadeUtils.HSVToRGB( HSVColor.Lerp( new HSVColor( startColor ), new HSVColor( endColor ), shiftTimer/shiftTime) );
			}

			RenderSettings.skybox.SetColor("_Tint", lerpColor);

			RenderSettings.fogColor = lerpColor;
			RenderSettings.fogDensity = fogDensity;

			// This doesn't look good without HSV transition
			HSVColor lerpHSV = new HSVColor( lerpColor );
			lerpHSV.v += lightBrightnessOffset;
			lerpHSV.s += lightSaturationOffset;

			light.color = lerpHSV.HSVToRGB();

			shiftTimer += Time.deltaTime;
		}
		else
		{
			shiftTimer = 0f;

			Color tempColor = startColor;
			startColor = endColor;
			endColor = tempColor;
		}
	}
}
