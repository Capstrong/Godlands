﻿using UnityEngine;
using System.Collections;

public class CollectAdjacentResources : LeafNode
{
	private GodInfo info;
	private GameObject gameObject;
	private Transform transform;

	public override void Init( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		info = gameObject.GetComponent<GodInfo>();
	}

	public override NodeStatus Tick()
	{
		foreach ( GameObject resource in GameObject.FindGameObjectsWithTag( "Resource" ) )
		{
			float distanceSquared = ( resource.GetComponent<Transform>().position - transform.position ).sqrMagnitude;
			if ( distanceSquared < ( info.resourceCollectionDistance * info.resourceCollectionDistance ) )
			{
				// collect resource
				++info.resources;
				GameObject.Destroy( resource );
				return NodeStatus.SUCCESS;
			}
		}

		return NodeStatus.FAILURE;
	}
}

/**
 * @brief Check if there are any resources left in the world.
 *
 * @details
 *     Status is SUCCESS if resources exist, FAILURE otherwise.
 */
public class ResourcesPresent : LeafNode
{
	public override void Init( Hashtable data )
	{
	}

	public override NodeStatus Tick()
	{
		if ( GameObject.FindGameObjectsWithTag( "Resource" ).Length > 0 )
		{
			return NodeStatus.SUCCESS;
		}
		else
		{
			return NodeStatus.FAILURE;
		}
	}
}

public class ChooseResourceTarget : LeafNode
{
	private GameObject gameObject;
	private Transform transform;
	private BehaviorTreeInfo info;

	public override void Init( Hashtable data )
	{
		gameObject = (GameObject)data["gameObject"];
		transform = gameObject.GetComponent<Transform>();
		info = gameObject.GetComponent<BehaviorTreeInfo>();
	}

	public override NodeStatus Tick()
	{
		NodeStatus status = NodeStatus.FAILURE;
		Resource[] gameObjects = GameObject.FindObjectsOfType<Resource>();

		if ( gameObjects.Length > 0 )
		{
			info.destination = gameObjects[0].GetComponent<Transform>().position;
		}

		for ( int index = 1; index < gameObjects.Length; ++index )
		{
			status = NodeStatus.SUCCESS;
			Transform resourceTransform = gameObjects[index].GetComponent<Transform>();
			if ( ( transform.position - resourceTransform.position ).sqrMagnitude <
			     ( transform.position - info.destination ).sqrMagnitude )
			{
				info.destination = resourceTransform.position;
			}
		}
		return status;
	}
}
