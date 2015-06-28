using UnityEngine;
using System.Collections;

public enum Level
{
	Invalid,
	ControlsScreen,
	MainGame,
	GoodEnding,
	BadEnding,
	MainMenu,
	Credits,
}

public static class LevelUtils
{
	public
	static string[] levels = {"",
	                          "ControlsScreen",
	                          "BlockOut",
	                          "goodEnding",
	                          "badEnding",
	                          "MainMenu",
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX // Disable credits on mac because they don't work for some reason
	                          "MainMenu",
#else
	                          "Credits",
#endif
	                          };

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
