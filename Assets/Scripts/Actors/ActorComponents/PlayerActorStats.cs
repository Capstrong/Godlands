using UnityEngine;
using System.Collections;

public class PlayerActorStats : ActorComponent {

	[SerializeField] float _startingMaxStamina = 0.0f;
	[SerializeField] float _staminaMaxIncrement = 0.0f;
	[SerializeField] float _staminaUseRate = 0.0f; // Stamina per second
	[SerializeField] float _staminaRechargeRate = 0.0f;
	[SerializeField] float _currMaxStamina = 0.0f;
	[SerializeField] float _currStamina = 0.0f;
	[SerializeField] bool _isUsingStamina = false;

	public override void Awake()
	{
		base.Awake();
		_currMaxStamina = _startingMaxStamina;
	}

	public void IncrementMaxStamina()
	{
		_currMaxStamina += _staminaMaxIncrement;
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
			_currStamina += _staminaRechargeRate;

			if (_currStamina > _currMaxStamina)
			{
				_currStamina = _currMaxStamina;
			}
		}
	}

}
