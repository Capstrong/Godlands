using UnityEngine;
using System.Collections;
using BehaviorTree;

public enum Comparison
{
	GreaterThan,
	EqualTo,
	LessThan
}

public class CheckHunger : LeafNode
{
	public Comparison comparison;
	[Range( 0.0f, 1.0f )]
	public float value;

	private float _hunger;

	public override void InitSelf( Hashtable data )
	{
		GameObject gameObject = (GameObject)data["gameObject"];
		_hunger = gameObject.GetComponent<BuddyStats>().hunger;
	}

	public override NodeStatus TickSelf()
	{
		switch ( comparison )
		{
			case Comparison.GreaterThan:
				if ( _hunger > value ) return NodeStatus.SUCCESS;
				break;
			case Comparison.EqualTo:
				if ( _hunger == value ) return NodeStatus.SUCCESS;
				break;
			case Comparison.LessThan:
				if ( _hunger < value ) return NodeStatus.SUCCESS;
				break;
		}

		return NodeStatus.FAILURE;
	}
}
