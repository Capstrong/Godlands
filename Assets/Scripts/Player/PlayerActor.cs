using UnityEngine;
using System.Collections;

[RequireComponent( typeof( ActorCamera ), typeof( PlayerActorStats) )]
public class PlayerActor : Actor
{
	ActorCamera _actorCamera = null;
	PlayerActorStats _actorStats = null;
	PlayerActorInventory _playerActorResources = null;
	PlayerControls _controls = null;
	Cutting _cutting = null;

	public ActorCamera actorCamera
	{
		get { return _actorCamera; }
	}

	public PlayerActorInventory inventory
	{
		get { return _playerActorResources; }
	}

	public PlayerActorStats stats
	{
		get { return _actorStats; }
	}

	public PlayerControls controls
	{
		get { return _controls; }
	}

	public Cutting cutting
	{
		get { return _cutting; }
	}

	public override void Awake()
	{
		base.Awake();

		_actorCamera = GetComponent<ActorCamera>();
		_actorStats = GetComponent<PlayerActorStats>();
		_playerActorResources = GetComponent<PlayerActorInventory>();
		_controls = GetComponent<PlayerControls>();
		_cutting = GetComponent<Cutting>();
	}
}
