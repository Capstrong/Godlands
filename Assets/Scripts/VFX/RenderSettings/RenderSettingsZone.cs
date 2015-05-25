using UnityEngine;
using System.Collections;

public class RenderSettingsZone : MonoBehaviour 
{
	[SerializeField] TimeLightingSettingsData _daySettings = null;
	[SerializeField] TimeLightingSettingsData _nightSettings = null;

	public LightingSettings lightingSettings
	{
		get
		{
			return new LightingSettings( _daySettings.timeSettings, _nightSettings.timeSettings );
		}
	}

	[SerializeField] float _shiftTime = 15f;
	
	void OnTriggerEnter( Collider otherCol )
	{
		if ( otherCol.GetComponentInParent<PlayerActor>() )
		{
			RenderSettingsManager.TransitionRenderSettings( lightingSettings, _shiftTime );
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon( transform.position, "EnviroZone.png", true );
	}
}
