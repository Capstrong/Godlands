using UnityEngine;
using System.Collections;

[RequireComponent( typeof( ActorCamera ) )]
public class PlayerActor : Actor
{
	ActorCamera _actorCamera;
	PlayerActorInventory _playerActorResources;

	public override void Awake()
	{
		base.Awake();

		_actorCamera = GetComponent<ActorCamera>();
		_playerActorResources = GetComponent<PlayerActorInventory>();
	}

	public ActorCamera actorCamera
	{
		get { return _actorCamera; }
	}

	public PlayerActorInventory actorResources
	{
		get { return _playerActorResources; }
	}
}
