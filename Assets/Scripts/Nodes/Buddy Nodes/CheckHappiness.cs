using UnityEngine;
using System.Collections;

using BehaviorTree;

public class CheckHappiness : LeafNode
{
	[Range( 0.0f, 1.0f )]
	public float value;
	public Comparison comparison;

	private BuddyStats _buddyStats;

	public override void InitSelf( Hashtable data )
	{
		GameObject gameObject = (GameObject)data["gameObject"];
		_buddyStats = gameObject.GetComponent<BuddyStats>();
	}

	public override NodeStatus TickSelf()
	{
		switch ( comparison )
		{
		case Comparison.EqualTo:
			if ( value == _buddyStats.happiness ) return NodeStatus.SUCCESS;
			break;
		case Comparison.GreaterThan:
			if ( value > _buddyStats.happiness ) return NodeStatus.SUCCESS;
			break;
		case Comparison.LessThan:
			if ( value < _buddyStats.happiness ) return NodeStatus.SUCCESS;
			break;
		}

		return NodeStatus.FAILURE;
	}
}
