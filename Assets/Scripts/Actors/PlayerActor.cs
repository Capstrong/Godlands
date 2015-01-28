using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ActorCamera))]
public class PlayerActor : Actor
{
	ActorCamera _actorCamera;

	public override void Awake()
	{
		base.Awake();

		_actorCamera = GetComponent<ActorCamera>();
	}

	public ActorCamera actorCamera
	{
		get { return _actorCamera; }
	}
}
