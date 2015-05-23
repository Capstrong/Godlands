using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RenderSettingsManager))]
public class RenderSettingsManagerEditor : Editor 
{
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		if( GUILayout.Button( "Display nearest render volume" ) )
		{
			RenderSettingsManager.SetToNearestZone( Camera.main.GetComponent<Transform>().position );
		}
	}
}