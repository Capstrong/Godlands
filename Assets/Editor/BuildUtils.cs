using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEditor;

public class BuildUnityPlayer : ScriptableObject
{
	public string gameName; // TODO get this automatically from project name or something?
	public Version version;
	public BuildSettings[] buildSettings;
	public string[] scenes; // TODO create a more safe system for selecting scenes.

	[HideInInspector]
	[SerializeField] Version _lastVersion;

	public void PerformBuild()
	{
		// reset patch number if major or minor was changed
		if ( version.major != _lastVersion.major || version.minor != _lastVersion.minor )
		{
			version.patch = 0;
		}

		foreach ( BuildSettings build in buildSettings )
		{
			string buildName = gameName + "_" + version + "_" + BuildSuffix( build.target );
			EditorUserBuildSettings.SwitchActiveBuildTarget( build.target );
			BuildPipeline.BuildPlayer(
				( from scene in scenes select "Assets/Scenes/" + scene + ".unity" ).ToArray<string>(),
				"Build/" + buildName + "/" + gameName + BuildExtension( build.target ),
				build.target,
				build.options );
		}

		_lastVersion = version;
		version.patch++;
	}

	/**
	 * @todo Add suffixes for different targets.
	 */
	static string BuildSuffix( BuildTarget target )
	{
		switch ( target )
		{
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "WIN";
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
				return "OSX";
			case BuildTarget.StandaloneLinux:
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
				return "LINUX";
			default:
				return "BUILD";
		}
	}

	static string BuildExtension( BuildTarget target )
	{
		switch ( target )
		{
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return ".exe";
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
				return ".app";
			case BuildTarget.StandaloneLinux:
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
				return ".LINUX"; // TODO what even is
			default:
				return ".build";
		}
	}

	/**
	 * @brief Add a menu item in the editor for creating new behavior tree assets.
	 *
	 * @todo
	 *     Create new asset in currently selected folder of the project view,
	 *     rather than always placing them in the root Assets folder.
	 *
	 * @todo
	 *     Check if asset already exists, because by default the AssetDatabase
	 *     will overwrite an existing asset of the same name.
	 */
	[MenuItem( "Assets/Create/Build Settings" )]
	public static void CreateBuildAsset()
	{
		BuildUnityPlayer buildAsset = ScriptableObject.CreateInstance<BuildUnityPlayer>();
		AssetDatabase.CreateAsset( buildAsset, "Assets/BuildSettings.asset" );
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = buildAsset;
	}
}

[Serializable]
public struct BuildSettings
{
	public BuildTarget target;
	public BuildOptions options;
}

[Serializable]
public struct Version
{
	public int major;
	public int minor;
	[HideInInspector]
	public int patch;
	public string extra;

	public static implicit operator string ( Version version )
	{
		return version.major + "." +
		       version.minor + "." +
		       version.patch +
		       ( version.extra != "" ? "-" + version.extra : "" );
	}
}
