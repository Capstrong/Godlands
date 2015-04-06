using UnityEngine;
using System.Collections;
using BehaviorTree;

public class IsTargetWithinRange : LeafNode
{
	public float minDistance;
	public float maxDistance;

	Transform _transform;
	Transform _target;

	public override void InitSelf( Hashtable data )
	{
		_transform = ( (GameObject)data["gameObject"] ).GetComponent<Transform>();
		_target = (Transform)data["target"];
	}

	public override NodeStatus TickSelf()
	{
		float distanceSqr = ( _transform.position - _target.position ).sqrMagnitude;
		if ( distanceSqr > minDistance * minDistance && distanceSqr < maxDistance * maxDistance )
		{
			return NodeStatus.SUCCESS;
		}
		else
		{
			return NodeStatus.FAILURE;
		}
	}
}
