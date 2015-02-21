using UnityEngine;
using System.Collections;

public class DebugUtils
{
	public static void Assert( bool condition, string message = "" )
	{
		if ( !condition )
		{
			Debug.LogError( message );
			// Debug.Break(); // Uncomment if you want to pause the editor when an assert is hit.
		}
	}
}
