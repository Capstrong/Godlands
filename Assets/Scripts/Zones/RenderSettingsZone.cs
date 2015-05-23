using UnityEngine;
using System.Collections;

public class RenderSettingsZone : MonoBehaviour 
{
	public RenderSettingsData renderSettings = new RenderSettingsData();
	[SerializeField] float _shiftTime = 15f;
	
	void OnTriggerEnter( Collider otherCol )
	{
		if ( otherCol.GetComponentInParent<PlayerActor>() )
		{
			RenderSettingsManager.TransitionRenderSettings( renderSettings, _shiftTime );
		}
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon( transform.position, "EnviroZone.png", true );
	}
}
