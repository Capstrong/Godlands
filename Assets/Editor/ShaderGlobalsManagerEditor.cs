using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ShaderGlobalsManager))]
public class ShaderGlobalsManagerEditor : Editor 
{
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		ShaderGlobalsManager script = (ShaderGlobalsManager)target;
		if( GUI.changed )
		{
			script.UpdateProperties();
		}
	}
}
