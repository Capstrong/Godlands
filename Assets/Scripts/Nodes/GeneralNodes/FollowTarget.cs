using UnityEngine;
using System.Collections;

using BehaviorTree;

public class FollowTarget : LeafNode
{
	private GameObject gameObject;
	private Transform transform;
	private BehaviorPhysicsController controller;
	private BehaviorTreeInfo info;

	public override void InitSelf( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		info = gameObject.GetComponent<BehaviorTreeInfo>();
		controller = gameObject.GetComponent<BehaviorPhysicsController>();
	}

	public override NodeStatus TickSelf()
	{
		controller.moveDirection = ( info.followTarget.position - transform.position ).normalized;

		if ( Vector3.Distance( transform.position, info.followTarget.position ) < 0.5f )
		{
			controller.moveDirection = Vector3.zero;
			return NodeStatus.SUCCESS;
		}
		else
		{
			return NodeStatus.RUNNING;
		}
	}
}
