using UnityEngine;
using System.Collections;

public class RenderSettingsZone : MonoBehaviour 
{
	[SerializeField] RenderSettingsData renderSettings;
	
	void OnTriggerEnter( Collider otherCol )
	{
		if ( otherCol.GetComponent<PlayerActor>() )
		{
			RenderSettingsManager.ChangeTargetRenderSettings( renderSettings );
		}
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon( transform.position, "EnviroZone.png", true );
	}
}
