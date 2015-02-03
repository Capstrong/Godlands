using UnityEngine;
using System.Collections;

[RequireComponent( typeof( ActorCamera ), typeof( PlayerActorStats) )]
public class PlayerActor : Actor
{
	ActorCamera _actorCamera = null;
	PlayerActorStats _actorStats = null;
	PlayerActorInventory _playerActorResources = null;

	public ActorCamera actorCamera
	{
		get { return _actorCamera; }
	}

	public PlayerActorInventory actorResources
	{
		get { return _playerActorResources; }
	}

	public PlayerActorStats actorStats
	{
		get { return _actorStats; }
	}

	public override void Awake()
	{
		base.Awake();

		_actorCamera = GetComponent<ActorCamera>();
		_actorStats = GetComponent<PlayerActorStats>();
		_playerActorResources = GetComponent<PlayerActorInventory>();
	}
}
