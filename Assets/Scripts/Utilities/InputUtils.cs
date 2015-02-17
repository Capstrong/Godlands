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

public class Button
{
	readonly string _buttonName;

	bool _buttonLast = false;
	bool _buttonDown = false;

	public Button( string buttonName )
	{
		_buttonName = buttonName;
	}

	public bool down
	{
		get { return _buttonDown; }
	}

	public bool up
	{
		get { return !_buttonDown; }
	}

	public bool pressed
	{
		get
		{
			return _buttonDown && !_buttonLast;
		}
	}

	public bool released
	{
		get { return !_buttonDown && _buttonLast; }
	}

	public void Update()
	{
		_buttonLast = _buttonDown;
		_buttonDown = InputUtils.GetButton( _buttonName );
	}
}
