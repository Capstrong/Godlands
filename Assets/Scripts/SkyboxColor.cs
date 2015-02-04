using UnityEngine;
using System.Collections;

public class SkyboxColor : MonoBehaviour 
{
	[SerializeField] Color startColor;
	[SerializeField] Color endColor;

	[SerializeField] float shiftTime = 5f;
	float shiftTimer = 0f;

	void Update()
	{
		if(shiftTimer < shiftTime)
		{
			Color lerpColor = Color.Lerp( startColor, endColor, shiftTimer/shiftTime );
			//Color lerpColor = WadeUtils.HSVToRGB( HSVColor.Lerp( new HSVColor( startColor ), new HSVColor( endColor ), shiftTimer/shiftTime) );
			RenderSettings.skybox.SetColor("_Tint", lerpColor);
			RenderSettings.fogColor = lerpColor;

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
