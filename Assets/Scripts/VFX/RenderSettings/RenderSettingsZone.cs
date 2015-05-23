using UnityEngine;
using System.Collections;

public class RenderSettingsZone : MonoBehaviour 
{
	[SerializeField] TimeLightingSettingsData _dayLightingSettings = null;
	[SerializeField] TimeLightingSettingsData _nightLightingSettings = null;
	[SerializeField] float _shiftTime = 15f;
	
	void OnTriggerEnter( Collider otherCol )
	{
		if ( otherCol.GetComponentInParent<PlayerActor>() )
		{
			RenderSettingsManager.TransitionRenderSettings( new LightingSettings( _dayLightingSettings.GetTimeLightingSettings(), 
			                                                                      _nightLightingSettings.GetTimeLightingSettings() ), _shiftTime );
		}
	}

	public LightingSettings GetLightingSettings()
	{
		return new LightingSettings( _dayLightingSettings.GetTimeLightingSettings(), 
		                             _nightLightingSettings.GetTimeLightingSettings() );
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon( transform.position, "EnviroZone.png", true );
	}
}
