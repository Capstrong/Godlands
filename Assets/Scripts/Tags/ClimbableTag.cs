using UnityEngine;
using System.Collections;

public class ClimbableTag : MonoBehaviour 
{
	[SerializeField] bool _xMovement = true;
	public bool xMovement
	{
		get { return _xMovement; }
	}

	[SerializeField] bool _yMovement = true;
	public bool yMovement
	{
		get { return _yMovement; }
	}

	void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube( Vector3.zero, Vector3.one );

		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.DrawIcon( transform.position, "ClimbVolume.png", true );
		Gizmos.DrawIcon( transform.position + transform.up * transform.localScale.y/2f, "Top.png", true );
	}
}
