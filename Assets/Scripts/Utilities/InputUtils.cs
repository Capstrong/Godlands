using UnityEngine;
using System.Collections;

public static class InputUtils
{
	// TODO ask Chris what this is even for
	public static void CheckForController()
	{
		if ( Input.GetJoystickNames().Length > 0 )
		{
			#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
				PlatformUtils.platformName = "_WIN";
			#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
				PlatformUtils.platformName = "_OSX";
			#endif
		}
		else
		{
			PlatformUtils.platformName = "_MOUSE";
		}
	}

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
