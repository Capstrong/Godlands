using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

	[ReadOnly("Target Transform")]
	[SerializeField] Transform _targetTransform = null;

	// Use this for initialization
	void Start () {
		_targetTransform = GetComponentInChildren<TeleportTargetTag>().gameObject.GetComponent<Transform>();
	}

	void OnTriggerEnter( Collider col )
	{
		PlayerControls controls = col.gameObject.GetComponent<PlayerControls>();

		if ( controls != null )
		{
			controls.Teleport( _targetTransform.position, _targetTransform.rotation );
		}
	}
}
