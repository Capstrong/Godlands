using UnityEngine;
using System.Collections;

using BehaviorTree;

public class PickRandomDestinationInRadius : LeafNode
{
	public float minRadius = 5.0f;
	public float maxRadius = 10.0f;

	private GameObject gameObject;
	private Transform transform;
	private BehaviorTreeInfo baseInfo;

	public override void InitSelf( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		baseInfo = gameObject.GetComponent<BehaviorTreeInfo>();
	}

	public override NodeStatus TickSelf()
	{
		Vector3 offset =
			Random.insideUnitCircle * Random.Range( minRadius, maxRadius );
		offset.z = offset.y;
		offset.y = 0;
		baseInfo.destination = transform.position + offset;

		return NodeStatus.SUCCESS;
	}
}
