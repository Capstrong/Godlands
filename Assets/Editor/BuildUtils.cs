using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

public class BuildUnityPlayer : MonoBehaviour
{
	[MenuItem( "Export/Build All" )]
	public static void PerformBuild()
	{
		// the scenes we want to include in the build
		string[] scenes = {"Scenes/BlockOut.unity"};

		DateTime currentDate = DateTime.Now;
		string buildName = "Godlands_" + currentDate.ToShortDateString();

		// build for windows stand alone
		string windowsStandAloneBuildName = buildName + "_WIN.exe";
		EditorUserBuildSettings.SwitchActiveBuildTarget( BuildTarget.StandaloneWindows );
		BuildPipeline.BuildPlayer( scenes, windowsStandAloneBuildName, BuildTarget.StandaloneWindows, BuildOptions.None );

		// build for windows stand alone
		string macStandAloneBuildName = buildName + "_OSX.exe";
		EditorUserBuildSettings.SwitchActiveBuildTarget( BuildTarget.StandaloneOSXUniversal );
		BuildPipeline.BuildPlayer( scenes, macStandAloneBuildName, BuildTarget.StandaloneOSXUniversal, BuildOptions.None );

		// build for windows stand alone
		string linuxStandAloneBuildName = buildName + "_LINUX.exe";
		EditorUserBuildSettings.SwitchActiveBuildTarget( BuildTarget.StandaloneLinuxUniversal );
		BuildPipeline.BuildPlayer( scenes, linuxStandAloneBuildName, BuildTarget.StandaloneLinuxUniversal, BuildOptions.None );
	}
}
