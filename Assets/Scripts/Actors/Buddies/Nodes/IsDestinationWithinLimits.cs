using UnityEngine;
using System.Collections;

using BehaviorTree;

public class IsDestinationWithinLimits : LeafNode
{
	private Hashtable _data;

	public override void InitSelf( Hashtable data )
	{
		_data = data;
	}

	public override NodeStatus TickSelf()
	{
		if ( MathUtils.IsWithinInfiniteVerticalCylinders( (Vector3) _data["destination"], LimitsManager.colliders ) )
		{
			return NodeStatus.SUCCESS;
		}

		return NodeStatus.FAILURE;
	}
}
