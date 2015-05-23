using UnityEngine;
using System.Collections;

using BehaviorTree;

public class FindGod : LeafNode
{
	private Hashtable _data;

	public override void InitSelf( Hashtable data )
	{
		_data = data;
	}

	public override NodeStatus TickSelf()
	{
		GodTag targetGod = GameObject.FindObjectOfType<GodTag>();

		if ( targetGod )
		{
			_data[ "god" ] = targetGod.GetComponent<Transform>();
			return NodeStatus.SUCCESS;
		}

		return NodeStatus.FAILURE;
	}
}
