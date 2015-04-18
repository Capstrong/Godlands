using UnityEngine;
using System.Collections;

using BehaviorTree;

public class IsTargetWithinLimits : LeafNode
{
	Transform _target;

	public override void InitSelf( Hashtable data )
	{
		_target = (Transform)data["target"];
	}

	public override NodeStatus TickSelf()
	{
		if ( MathUtils.IsWithinInfiniteVerticalCylinders( _target.position, LimitsManager.colliders ) )
		{
			return NodeStatus.SUCCESS;
		}

		return NodeStatus.FAILURE;
	}
}
