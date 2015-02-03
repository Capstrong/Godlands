using UnityEngine;
using System.Collections;

public class ClimbableTag : MonoBehaviour 
{
	[SerializeField] bool xMovement;
	public bool XMovement
	{
		get { return xMovement; }
	}

	[SerializeField] bool yMovement;
	public bool YMovement
	{
		get { return yMovement; }
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
