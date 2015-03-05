using UnityEngine;
using System.Collections;

public class BehaviorTreeInfo : MonoBehaviour
{
	public Vector3 destination;

	public Transform followTarget;

	public float moveSpeed;
	public float watchDistance;

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere( transform.position, watchDistance );
	}
}
