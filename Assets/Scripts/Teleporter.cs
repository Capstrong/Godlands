using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

	Transform _targetTransform = null;
	[SerializeField] string _name = "";
	[SerializeField] AudioSource _teleportSound = null;

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
		if ( !col.gameObject.transform.parent )
		{
			return;
		}

		// Get to the player from the bumper
		PlayerControls controls = col.gameObject.GetComponentInParent<PlayerControls>();

		if ( controls )
		{
			controls.Teleport( _targetTransform.position, _targetTransform.rotation, false );
			SoundManager.Play2DSound( _teleportSound );
		}
	}
}
