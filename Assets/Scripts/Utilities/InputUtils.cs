using UnityEngine;
using System.Collections;

public static class InputUtils
{
	public static bool GetButton( string buttonName )
	{
		return PositiveAxisInput( buttonName );
	}

	public static float GetAxis( string axisName )
	{
		return Input.GetAxis( axisName + PlatformUtils.platformName );
	}

	public static bool PositiveAxisInput(string axisName)
	{
		return Input.GetAxis(axisName + PlatformUtils.platformName) > WadeUtils.SMALLNUMBER;
	}

	public static bool NegativeAxisInput(string axisName)
	{
		return Input.GetAxis(axisName + PlatformUtils.platformName) < -WadeUtils.SMALLNUMBER;
	}
}

public struct Button
{
	readonly string _buttonName;

	bool _buttonLast;
	bool _buttonDown;

	public Button( string buttonName )
	{
		_buttonName = buttonName;

		_buttonLast = false;
		_buttonDown = false;
	}

	public static implicit operator bool( Button button )
	{
		return button._buttonDown;
	}

	public bool down
	{
		get
		{
			return _buttonDown && !_buttonLast;
		}
	}

	public bool up
	{
		get { return !_buttonDown && _buttonLast; }
	}

	public void Update()
	{
		_buttonLast = _buttonDown;
		_buttonDown = InputUtils.GetButton( _buttonName );
	}
}

public struct AxisButtons
{
	enum ButtonState
	{
		Invalid,
		Positive,
		Negative,
		Neutral,
	}

	readonly string _buttonName;

	ButtonState _currentState;
	ButtonState _lastState;

	public AxisButtons( string buttonName )
	{
		_buttonName = buttonName;

		_currentState = ButtonState.Neutral;
		_lastState = ButtonState.Neutral;
	}

	public bool positiveDown
	{
		get
		{
			return ( _currentState == ButtonState.Positive ) &&  ( _lastState != ButtonState.Positive );
		}
	}

	public bool negativeDown
	{
		get
		{
			return ( _currentState == ButtonState.Negative ) &&  ( _lastState != ButtonState.Negative );
		}
	}

	public void Update()
	{
		_lastState = _currentState;

		float axisInput = InputUtils.GetAxis( _buttonName );

		if ( WadeUtils.IsPositive( axisInput ) )
		{
			_currentState = ButtonState.Positive;
		}
		else if ( WadeUtils.IsNegative( axisInput ) )
		{
			_currentState = ButtonState.Negative;
		}
		else
		{
			_currentState = ButtonState.Neutral;
		}
	}
}
