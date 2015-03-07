using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

	Transform _targetTransform = null;
	[SerializeField] string _name = "";

	void Start () {
		_targetTransform = GetComponentInChildren<TeleportTargetTag>().gameObject.GetComponent<Transform>();

		TextMesh[] textMeshes =  GetComponentsInChildren<TextMesh>();

		foreach ( TextMesh textMesh in textMeshes )
		{
			textMesh.text =_name;
		}
	}

	void OnTriggerEnter( Collider col )
	{
		if ( col.gameObject.transform.parent == null )
		{
			return;
		}

		// Get to the player from the bumper
		PlayerControls controls = col.gameObject.GetComponent<Transform>().parent.gameObject.GetComponent<PlayerControls>();

		if ( controls != null )
		{
			controls.Teleport( _targetTransform.position, _targetTransform.rotation );
		}
	}
}
