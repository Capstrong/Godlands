using UnityEngine;
using System.Collections;

using BehaviorTree;

public class Idle : LeafNode
{
	private float _time;

	public override void InitSelf( Hashtable data )
	{
		BuddyInfo info = ( (GameObject)data["gameObject"] ).GetComponent<BuddyInfo>();
		_time = Random.Range( info.minIdleTime, info.maxIdleTime );
	}

	public override NodeStatus TickSelf()
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