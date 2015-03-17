using UnityEngine;
using System.Collections;

using BehaviorTree;

public class PickRandomDestinationInRadius : LeafNode
{
	private GameObject gameObject;
	private Transform transform;
	private BehaviorTreeInfo baseInfo;
	private BuddyInfo buddyInfo;

	public override void InitSelf( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		baseInfo = gameObject.GetComponent<BehaviorTreeInfo>();
		buddyInfo = gameObject.GetComponent<BuddyInfo>();
	}

	public override NodeStatus TickSelf()
	{
		Vector3 offset = Random.insideUnitCircle
			* Random.Range( buddyInfo.minIdleWalkRadius, buddyInfo.maxIdleWalkRadius );
		offset.z = offset.y;
		offset.y = 0;
		baseInfo.destination = transform.position + offset;

		return NodeStatus.SUCCESS;
	}
}
