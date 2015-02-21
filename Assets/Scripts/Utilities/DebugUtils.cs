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

// Attribute that displays variable in inspector for debugging purposes but does not make it editable
// Usage:
// [DisplayOnly] float foo;
public class DisplayOnlyAttribute : PropertyAttribute
{

}
