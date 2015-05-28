using UnityEngine;
using System.Collections;

using BehaviorTree;

public class Idle : LeafNode
{
	public float time;

	private float _time;

	public override void InitSelf( Hashtable data )
	{
		_time = time;
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