using UnityEngine;
using System.Collections;

using BehaviorTree;

public class StopMoving : LeafNode
{
	private GameObject gameObject;
	private BehaviorPhysicsController controller;

	public override void InitSelf( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		controller = gameObject.GetComponent<BehaviorPhysicsController>();
	}

	public override NodeStatus TickSelf()
	{
		controller.moveDirection = Vector3.zero;
		return NodeStatus.SUCCESS;
	}
}
