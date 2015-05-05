using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEditor;

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


