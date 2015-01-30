using UnityEngine;
using System.Collections;

public class ClimbableTag : MonoBehaviour 
{
	void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		Gizmos.DrawIcon(transform.position, "ClimbVolume.png", true);
	}
}
