using UnityEngine;
using System.Collections;

public class DebugUtils
{
	public static void Assert( bool condition, string message = "" )
	{
		if ( !condition )
		{
			Debug.LogError( message );
		}
	}
}
