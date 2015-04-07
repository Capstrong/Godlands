using UnityEngine;
using System.Collections;

using BehaviorTree;

public class PickRandomDestinationInRadius : LeafNode
{
	public float minRadius = 5.0f;
	public float maxRadius = 10.0f;

	private Transform _transform;
	private Vector3 _destination;

	public override void InitSelf( Hashtable data )
	{
		GameObject gameObject = (GameObject)data["gameObject"];
		_transform = gameObject.GetComponent<Transform>();
		_destination = (Vector3)data["destination"];
	}

	public override NodeStatus TickSelf()
	{
		Vector3 offset =
			Random.insideUnitCircle * Random.Range( minRadius, maxRadius );
		offset.z = offset.y;
		offset.y = 0;
		_destination = _transform.position + offset;

		return NodeStatus.SUCCESS;
	}
}
