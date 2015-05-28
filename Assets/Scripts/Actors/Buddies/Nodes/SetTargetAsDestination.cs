using UnityEngine;
using System.Collections;

using BehaviorTree;

public class SetTargetAsDestination : LeafNode
{
	Hashtable _data;

	public override void InitSelf( Hashtable data )
	{
		_data = data;
	}

	public override NodeStatus TickSelf()
	{
		_data["destination"] = ( (Transform)_data["target"] ).position;
		return NodeStatus.SUCCESS;
	}
}
