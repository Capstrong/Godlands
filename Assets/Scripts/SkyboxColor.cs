using UnityEngine;
using System.Collections;

public class SkyboxColor : MonoBehaviour 
{
	[SerializeField] Color startColor;
	[SerializeField] Color endColor;

	[SerializeField] bool shiftColor = false;
	[SerializeField] bool useHSV = true;

	[SerializeField] Light light;

	[SerializeField] [Range(-2f, 2f)]
	float lightBrightnessOffset = 1.2f;

	[SerializeField] [Range(-1f, 0f)]
	float lightSaturationOffset = -0.3f;

	[SerializeField] [Range(0f, 0.05f)]
	float fogDensity = 0.008f;

	[SerializeField] [Range(1f, 30f)]
	float shiftTime = 30f;
	float shiftTimer = 0f;
	

	void Update()
	{
		Color lerpColor = new Color();

		if(shiftColor)
		{
			if(shiftTimer < shiftTime)
			{


				if(!useHSV)
				{
					lerpColor = Color.Lerp( startColor, endColor, shiftTimer/shiftTime );
				}
				else
				{
					lerpColor = WadeUtils.HSVToRGB( HSVColor.Lerp( new HSVColor( startColor ), new HSVColor( endColor ), shiftTimer/shiftTime) );
				}

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
		else
		{
			lerpColor = startColor;
		}

		RenderSettings.skybox.SetColor("_Tint", lerpColor);
		
		RenderSettings.fogColor = lerpColor;
		RenderSettings.fogDensity = fogDensity;
		
		// This doesn't look good without HSV transition
		HSVColor lerpHSV = new HSVColor( lerpColor );
		lerpHSV.v += lightBrightnessOffset;
		lerpHSV.s += lightSaturationOffset;
		
		light.color = lerpHSV.HSVToRGB();
		

	}
}
