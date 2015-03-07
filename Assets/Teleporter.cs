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
		Debug.Log("Trigger entered");

		if ( col.gameObject.transform.parent == null )
		{
			return;
		}

		PlayerControls controls = col.gameObject.transform.parent.gameObject.GetComponent<PlayerControls>();

		if ( controls != null )
		{
			Debug.Log("Teleporting");
			controls.Teleport( _targetTransform.position, _targetTransform.rotation );
		}
	}
}
