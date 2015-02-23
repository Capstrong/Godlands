﻿using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PlayerCamera ), typeof( PlayerStats) )]
public class PlayerActor : Actor
{
	PlayerCamera _actorCamera = null;
	PlayerStats _actorStats = null;
	PlayerInventory _playerActorResources = null;
	PlayerControls _controls = null;
	Cutting _cutting = null;

	public PlayerCamera actorCamera
	{
		get { return _actorCamera; }
	}

	public PlayerInventory inventory
	{
		get { return _playerActorResources; }
	}

	public PlayerStats stats
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

		_actorCamera = GetComponent<PlayerCamera>();
		_actorStats = GetComponent<PlayerStats>();
		_playerActorResources = GetComponent<PlayerInventory>();
		_controls = GetComponent<PlayerControls>();
		_cutting = GetComponent<Cutting>();
	}
}