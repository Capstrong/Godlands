using UnityEngine;
using System.Collections;

using BehaviorTree;

public class SetTrigger : LeafNode
{
	public string triggerName;

	private Animator _animator;

	public override void InitSelf( Hashtable data )
	{
		GameObject gameObject = (GameObject)data["gameObject"];
		_animator = gameObject.GetComponentInChildren<Animator>();
	}

	public override NodeStatus TickSelf()
	{
		_animator.SetTrigger( triggerName );
		return NodeStatus.SUCCESS;
	}
}
