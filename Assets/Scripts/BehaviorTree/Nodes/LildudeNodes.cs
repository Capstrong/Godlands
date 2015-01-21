using UnityEngine;
using System.Collections;

public class Idle : LeafNode
{
	private float _time;

	public override void Init( Hashtable data )
	{
		LildudeInfo info = ( (GameObject)data["gameObject"] ).GetComponent<LildudeInfo>();
		_time = Random.Range( info.minIdleTime, info.maxIdleTime );
	}

	public override NodeStatus Tick()
	{
		_time -= Time.deltaTime;
		if ( _time < 0.0f )
		{
			return NodeStatus.SUCCESS;
		}
		else
		{
			return NodeStatus.RUNNING;
		}
	}
}

public class PickRandomDestinationInRadius : LeafNode
{
	private GameObject gameObject;
	private Transform transform;
	private LildudeInfo info;

	public override void Init( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		info = gameObject.GetComponent<LildudeInfo>();
	}

	public override NodeStatus Tick()
	{
		Vector3 offset = Random.insideUnitCircle
			* Random.Range( info.minIdleWalkRadius, info.maxIdleWalkRadius );
		offset.z = offset.y;
		offset.y = 0;
		info.destination = transform.position + offset;

		return NodeStatus.SUCCESS;
	}
}

public class MoveToDestination : LeafNode
{
	private GameObject gameObject;
	private Transform transform;
	private LildudeInfo info;

	public override void Init( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		info = gameObject.GetComponent<LildudeInfo>();
	}

	public override NodeStatus Tick()
	{
		Vector3 translation = ( info.destination - transform.position ).normalized
			* info.moveSpeed * Time.deltaTime;
		transform.Translate( translation );

		if ( Vector3.Distance( transform.position, info.destination ) < 1.0f )
		{
			return NodeStatus.SUCCESS;
		}
		else
		{
			return NodeStatus.RUNNING;
		}
	}
}
