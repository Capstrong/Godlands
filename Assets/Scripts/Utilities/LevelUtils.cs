using UnityEngine;
using System.Collections;

public enum Level
{
	Invalid,
	ControlsScreen,
	MainGame,
}

public static class LevelUtils
{
	public static string[] levels = { "",
	                               "controlsScreen",
	                               "BlockOut"};

	public static void LoadLevel( Level level )
	{
		if ( level != Level.Invalid )
		{
			Application.LoadLevel( levels[(int)level] );
		}
		else
		{
			Debug.LogError( "Trying to load invalid level." );
		}
	}
}
