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
}

public static class LevelUtils
{
	public
	static string[] levels = {"",
	                          "ControlsScreen",
	                          "BlockOut",
	                          "goodEnding",
	                          "badEnding",
	                          "MainMenu"};

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
