using UnityEngine;
using System.Collections;

using BehaviorTree;

public class IsDestinationWithinLimits : LeafNode
{
	Vector3 _destination;

	public override void InitSelf( Hashtable data )
	{
		_destination = (Vector3)data["destination"];
	}

	public override NodeStatus TickSelf()
	{
		if ( MathUtils.IsWithinInfiniteVerticalCylinders( _destination, LimitsManager.colliders ) )
		{
			return NodeStatus.SUCCESS;
		}

		return NodeStatus.FAILURE;
	}
}
