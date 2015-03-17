using UnityEngine;
using System.Collections;

using BehaviorTree;

public class MoveToDestination : LeafNode
{
	private GameObject gameObject;
	private Transform transform;
	private BehaviorPhysicsController controller;
	private BehaviorTreeInfo info;

	public override void InitSelf( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		controller = gameObject.GetComponent<BehaviorPhysicsController>();
		info = gameObject.GetComponent<BehaviorTreeInfo>();
	}

	public override NodeStatus TickSelf()
	{
		controller.moveDirection = ( info.destination - transform.position ).normalized;

		Debug.DrawLine( transform.position, info.destination );

		if ( Vector3.Distance( transform.position, info.destination ) < 1.0f )
		{
			info.destination = Vector3.zero;
			controller.moveDirection = Vector3.zero;
			return NodeStatus.SUCCESS;
		}
		else
		{
			return NodeStatus.RUNNING;
		}
	}
}
