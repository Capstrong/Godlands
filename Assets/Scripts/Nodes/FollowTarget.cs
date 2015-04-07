using UnityEngine;
using System.Collections;

using BehaviorTree;

public class FollowTarget : LeafNode
{
	public float minDistance;

	private Transform _transform;
	private BehaviorPhysicsController _controller;
	private Transform _target;

	public override void InitSelf( Hashtable data )
	{
		GameObject gameObject = (GameObject)data["gameObject"];
		_transform = gameObject.GetComponent<Transform>();
		_controller = gameObject.GetComponent<BehaviorPhysicsController>();
		_target = (Transform)data["target"];
	}

	public override NodeStatus TickSelf()
	{
		_controller.moveDirection = ( _target.position - _transform.position ).normalized;

		if ( Vector3.Distance( _transform.position, _target.position ) < minDistance )
		{
			_controller.moveDirection = Vector3.zero;
			return NodeStatus.SUCCESS;
		}
		else
		{
			return NodeStatus.RUNNING;
		}
	}
}
