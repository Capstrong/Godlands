using UnityEngine;
using System.Collections;

using BehaviorTree;

public class MoveToDestination : LeafNode
{
	public float minDistance;

	private Transform _transform;
	private BehaviorPhysicsController _controller;
	private Vector3 _destination;

	public override void InitSelf( Hashtable data )
	{
		GameObject gameObject = (GameObject)data["gameObject"];
		_transform = gameObject.GetComponent<Transform>();
		_controller = gameObject.GetComponent<BehaviorPhysicsController>();
		_destination = (Vector3)data["destination"];
	}

	public override NodeStatus TickSelf()
	{
		_controller.moveDirection = ( _destination - _transform.position ).normalized;

		if ( ( _transform.position - _destination ).sqrMagnitude < minDistance * minDistance )
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
