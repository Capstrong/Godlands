using UnityEngine;
using System.Collections;

[RequireComponent( typeof( ActorCamera ) )]
public class PlayerActor : Actor
{
	ActorCamera _actorCamera;
	PlayerActorResources _playerActorResources;

	public override void Awake()
	{
		base.Awake();

		_actorCamera = GetComponent<ActorCamera>();
		_playerActorResources = GetComponent<PlayerActorResources>();
	}

	public ActorCamera actorCamera
	{
		get { return _actorCamera; }
	}

	public PlayerActorResources actorResources
	{
		get { return _playerActorResources; }
	}
}
