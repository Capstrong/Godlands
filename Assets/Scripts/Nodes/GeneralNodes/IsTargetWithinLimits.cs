﻿using UnityEngine;
using System.Collections;

using BehaviorTree;

public class IsTargetWithinLimits : LeafNode
{
	private GameObject _gameObject;
	private BehaviorTreeInfo _info;

	public override void InitSelf( Hashtable data )
	{
		_gameObject = (GameObject)data["gameObject"];
		_info = _gameObject.GetComponent<BehaviorTreeInfo>();
	}

	public override NodeStatus TickSelf()
	{
		if ( MathUtils.IsWithinInfiniteVerticalCylinders( _info.followTarget.position, LimitsManager.colliders ) )
		{
			return NodeStatus.SUCCESS;
		}
		else
		{
			return NodeStatus.FAILURE;
		}
	}
}
