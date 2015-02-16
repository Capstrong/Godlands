using UnityEngine;
using System.Collections;

public class RenderSettingsZone : MonoBehaviour 
{
	[SerializeField] RenderSettingsData _renderSettings = new RenderSettingsData();
	[SerializeField] float _shiftTime = 15f;
	
	void OnTriggerEnter( Collider otherCol )
	{
		Debug.Log("Poop");
		if ( otherCol.GetComponentInParent<PlayerActor>() )
		{
			Debug.Log("Poop2");
			RenderSettingsManager.ChangeTargetRenderSettings( _renderSettings, _shiftTime );
		}
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon( transform.position, "EnviroZone.png", true );
	}
}
