using UnityEngine;
using System.Collections;

public class Idle : LeafNode
{
	private float _time;

	public override void Init( Hashtable data )
	{
		BuddyInfo info = ( (GameObject)data["gameObject"] ).GetComponent<BuddyInfo>();
		_time = Random.Range( info.minIdleTime, info.maxIdleTime );
	}

	public override NodeStatus Tick()
	{
		_time -= Time.deltaTime;
		if ( _time < 0.0f )
		{
			return NodeStatus.SUCCESS;
		}
		else
		{
			return NodeStatus.RUNNING;
		}
	}
}

public class PickRandomDestinationInRadius : LeafNode
{
	private GameObject gameObject;
	private Transform transform;
	private BehaviorTreeInfo baseInfo;
	private BuddyInfo buddyInfo;

	public override void Init( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		baseInfo = gameObject.GetComponent<BehaviorTreeInfo>();
		buddyInfo = gameObject.GetComponent<BuddyInfo>();
	}

	public override NodeStatus Tick()
	{
		Vector3 offset = Random.insideUnitCircle
			* Random.Range( buddyInfo.minIdleWalkRadius, buddyInfo.maxIdleWalkRadius );
		offset.z = offset.y;
		offset.y = 0;
		baseInfo.destination = transform.position + offset;

		return NodeStatus.SUCCESS;
	}
}
