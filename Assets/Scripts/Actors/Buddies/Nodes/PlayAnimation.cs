using UnityEngine;
using System.Collections;

using BehaviorTree;

public class PlayAnimation : LeafNode
{
	public string animation;

	private Animator _animator;

	public override void InitSelf( Hashtable data )
	{
		GameObject gameObject = (GameObject)data["gameObject"];
		_animator = gameObject.GetComponentInChildren<Animator>();
	}

	public override NodeStatus TickSelf()
	{
		_animator.Play( animation );
		return NodeStatus.SUCCESS;
	}
}
