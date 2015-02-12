using UnityEngine;
using System.Collections;

public class RenderSettingsZone : MonoBehaviour 
{
	[SerializeField] RenderSettingsData _renderSettings = null;
	
	void OnTriggerEnter( Collider otherCol )
	{
		if ( otherCol.GetComponent<PlayerActor>() )
		{
			RenderSettingsManager.ChangeTargetRenderSettings( _renderSettings );
		}
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon( transform.position, "EnviroZone.png", true );
	}
}
