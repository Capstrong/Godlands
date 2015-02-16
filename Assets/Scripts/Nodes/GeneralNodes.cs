using UnityEngine;
using System.Collections;

public class MoveToDestination : LeafNode
{
	private GameObject gameObject;
	private Transform transform;
	private ActorPhysics actorPhysics;
	private BehaviorTreeInfo info;

	public override void Init( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		actorPhysics = gameObject.GetComponent<ActorPhysics>();
		info = gameObject.GetComponent<BehaviorTreeInfo>();
	}

	public override NodeStatus Tick()
	{
		actorPhysics.GroundMovement( ( info.destination - transform.position ).normalized );

		if ( Vector3.Distance( transform.position, info.destination ) < 1.0f )
		{
			info.destination = Vector3.zero;
			actorPhysics.ComeToStop();
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
	private ActorPhysics actorPhysics;
	private BehaviorTreeInfo info;

	public override void Init( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		info = gameObject.GetComponent<BehaviorTreeInfo>();
		actorPhysics = gameObject.GetComponent<ActorPhysics>();
	}

	public override NodeStatus Tick()
	{
		actorPhysics.GroundMovement( ( info.followTarget.position - transform.position ).normalized );

		if ( Vector3.Distance( transform.position, info.followTarget.position ) < 0.5f )
		{
			info.followTarget = null;
			actorPhysics.ComeToStop();
			return NodeStatus.SUCCESS;
		}
		else
		{
			return NodeStatus.RUNNING;
		}
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
