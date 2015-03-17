using UnityEngine;
using System.Collections;

using BehaviorTree;

public class SetTargetAsDestination : LeafNode
{
	private GameObject gameObject;
	private BehaviorTreeInfo info;
	private BehaviorPhysicsController controller;

	public override void InitSelf( Hashtable data )
	{
		gameObject = (GameObject) data["gameObject"];
		info = gameObject.GetComponent<BehaviorTreeInfo>();
		controller = gameObject.GetComponent<BehaviorPhysicsController>();
	}

	public override NodeStatus TickSelf()
	{
		if ( info.followTarget == null )
		{
			Debug.LogError("Follow target is null");
			return NodeStatus.FAILURE;
		}

		info.destination = info.followTarget.position;
		controller.moveDirection = Vector3.zero;
		return NodeStatus.SUCCESS;
	}
}
