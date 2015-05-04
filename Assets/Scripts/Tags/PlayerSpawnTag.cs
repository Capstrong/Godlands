using UnityEngine;
using System.Collections;

public class PlayerSpawnTag : MonoBehaviour
{
	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue.SetAlpha( 0.5f );
		Gizmos.matrix = GetComponent<Transform>().localToWorldMatrix;

		Gizmos.DrawCube( new Vector3( 0.0f, 1.0f, 0.0f ), new Vector3( 1.0f, 2.0f, 1.0f ) );
		Gizmos.DrawWireCube( new Vector3( 0.0f, 1.0f, 0.0f ), new Vector3( 1.0f, 2.0f, 1.0f ) );
	}
}
