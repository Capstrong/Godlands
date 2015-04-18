using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ShaderGlobalsManager))]
public class ShaderGlobalsManagerEditor : Editor 
{
	[SerializeField] bool previewGlobals = false;

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		if( !Application.isPlaying )
		{
			previewGlobals = EditorGUILayout.Toggle( "Preview Globals", previewGlobals );

			ShaderGlobalsManager script = (ShaderGlobalsManager)target;

			if( previewGlobals )
			{
				if( GUI.changed )
				{
					script.UpdateProperties();
				}
			}
			else if( GUI.changed )
			{
				script.SetDefaultProperties();
			}
		}
		else
		{
			ShaderGlobalsManager script = (ShaderGlobalsManager)target;
			if( GUI.changed )
			{
				script.UpdateProperties();
			}
		}
	}
}
