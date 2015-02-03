﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent( typeof(PlayerActorPhysics) )]
public class PlayerActorStats : ActorComponent {

	[SerializeField] float _startingMaxStamina = 0.0f;
	[SerializeField] float _staminaMaxIncrement = 0.0f;
	[SerializeField] float _staminaUseRate = 0.0f; // Stamina per second
	[SerializeField] float _staminaRechargeRate = 0.0f;
	[SerializeField] float _currMaxStamina = 0.0f;
	[SerializeField] float _currStamina = 0.0f;
	[SerializeField] bool _isUsingStamina = false;
	[SerializeField] Text _staminaText = null;

	PlayerActorPhysics _actorPhysics = null;

	public override void Awake()
	{
		base.Awake();
		_actorPhysics = GetComponent<PlayerActorPhysics>();
		_currMaxStamina = _startingMaxStamina;
	}

	public void IncrementMaxStamina()
	{
		_currMaxStamina += _staminaMaxIncrement;
	}

	public void DecrementMaxStamina()
	{
		_currMaxStamina = Mathf.Max(_currMaxStamina - _staminaMaxIncrement, 0.0f); // decrement and clamp at a minimum of 0
	}

	public bool CanUseStamina()
	{
		return ( _currStamina > 0.0f );
	}

	public void StartUsingStamina()
	{
		_isUsingStamina = true;
	}

	public void StopUsingStamina()
	{
		_isUsingStamina = false;
	}

	void Update()
	{
		if (_isUsingStamina)
		{
			_currStamina -= _staminaUseRate;

			if (_currStamina <= 0)
			{
				_currStamina = 0;

				StopUsingStamina();
			}
		}
		else
		{
			if ( !_actorPhysics.isGrabbing )
			{
				_currStamina += _staminaRechargeRate;

				if ( _currStamina > _currMaxStamina )
				{
					_currStamina = _currMaxStamina;
				}
			}
		}

		_staminaText.text = "Stamina: " + _currStamina;
	}

}
