using UnityEngine;
using System.Collections;

public class MoveToDestination : LeafNode
{
	private GameObject gameObject;
	private Transform transform;
	private BehaviorPhysicsController controller;
	private BehaviorTreeInfo info;

	public override void Init( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		controller = gameObject.GetComponent<BehaviorPhysicsController>();
		info = gameObject.GetComponent<BehaviorTreeInfo>();
	}

	public override NodeStatus Tick()
	{
		controller.moveDirection = ( info.destination - transform.position ).normalized;

		if ( Vector3.Distance( transform.position, info.destination ) < 1.0f )
		{
			info.destination = Vector3.zero;
			controller.moveDirection = Vector3.zero;
			return NodeStatus.SUCCESS;
		}
		else
		{
			return NodeStatus.RUNNING;
		}
	}
}

public class FollowTarget : LeafNode
{
	private GameObject gameObject;
	private Transform transform;
	private BehaviorPhysicsController controller;
	private BehaviorTreeInfo info;

	public override void Init( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		info = gameObject.GetComponent<BehaviorTreeInfo>();
		controller = gameObject.GetComponent<BehaviorPhysicsController>();
	}

	public override NodeStatus Tick()
	{
		controller.moveDirection = ( info.followTarget.position - transform.position ).normalized;

		if ( Vector3.Distance( transform.position, info.followTarget.position ) < 0.5f )
		{
			info.followTarget = null;
			controller.moveDirection = Vector3.zero;
			return NodeStatus.SUCCESS;
		}
		else
		{
			return NodeStatus.RUNNING;
		}
	}
}

public class IsTargetWithinLimits : LeafNode
{
	private GameObject _gameObject;
	private Transform _transform;
	private BehaviorPhysicsController _controller;
	private BehaviorTreeInfo _info;
	private Limits _limits;

	public override void Init( Hashtable data )
	{
		_gameObject = (GameObject)data["gameObject"];
		_transform = _gameObject.GetComponent<Transform>();
		_info = _gameObject.GetComponent<BehaviorTreeInfo>();
		_controller = _gameObject.GetComponent<BehaviorPhysicsController>();
		_limits = _gameObject.GetComponent<Limits>();
	}

	public override NodeStatus Tick()
	{
		//_controller.moveDirection = ( _info.followTarget.position - _transform.position ).normalized;

		//if ( Vector3.Distance( _transform.position, _info.followTarget.position ) < 0.5f )
		//{
		//	_info.followTarget = null;
		//	_controller.moveDirection = Vector3.zero;
		//	return NodeStatus.SUCCESS;
		//}
		//else
		//{
		//	return NodeStatus.RUNNING;
		//}

		return NodeStatus.SUCCESS;
	}
}

/**
 * @brief Check if any gods are within the watch distance.
 *
 * @details
 *     Status is FAILURE if no other gods, SUCCESS otherwise.
 */
public class GodsWithinWatchDistance : LeafNode
{
	private Transform transform;
	private BehaviorTreeInfo info;

	public override void Init( Hashtable data )
	{
		GameObject gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		info = gameObject.GetComponent<BehaviorTreeInfo>();
	}

	public override NodeStatus Tick()
	{
		foreach ( GodTag god in GameObject.FindObjectsOfType<GodTag>() )
		{
			if ( ( transform.position - god.GetComponent<Transform>().position )
				.sqrMagnitude < info.watchDistance * info.watchDistance )
			{
				return NodeStatus.SUCCESS;
			}
		}
		return NodeStatus.FAILURE;
	}
}

public class ChooseTargetGod : LeafNode
{
	private Transform transform;
	private BehaviorTreeInfo info;

	public override void Init( Hashtable data )
	{
		GameObject gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		info = gameObject.GetComponent<BehaviorTreeInfo>();
	}

	public override NodeStatus Tick()
	{
		foreach ( GodTag god in GameObject.FindObjectsOfType<GodTag>() )
		{
			if ( ( transform.position - god.GetComponent<Transform>().position )
				.sqrMagnitude < info.watchDistance * info.watchDistance )
			{
				info.followTarget = god.GetComponent<Transform>();
				return NodeStatus.SUCCESS;
			}
		}
		return NodeStatus.FAILURE;
	}
}
