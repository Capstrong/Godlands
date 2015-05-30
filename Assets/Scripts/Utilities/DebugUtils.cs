using UnityEngine;
using System.Collections;

public class DebugUtils
{
	public class AssertException : System.Exception
	{
		private string _message;

		public AssertException( string message )
		{
			_message = message;
		}

		public override string ToString()
		{
			return _message;
		}
	}

	public static void Assert( bool condition, string message = "" )
	{
		if ( !condition )
		{
			// throw an exception to ensure that exection
			// does not continue after assert is hit.
			Debug.LogError( message );
			throw new AssertException( message );
			// Debug.Break(); // Uncomment if you want to pause the editor when an assert is hit.
		}
	}
}
