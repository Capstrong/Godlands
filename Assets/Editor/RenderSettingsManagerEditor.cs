using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RenderSettingsManager))]
public class RenderSettingsManagerEditor : Editor 
{
	RenderSettingsZone _renderSettingsZone = null;
	TimeLightingSettingsData _dayLightingSettings = null;
	TimeLightingSettingsData _nightLightingSettings = null;

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		if( GUILayout.Button( "Display nearest render volume" ) )
		{
			RenderSettingsManager.SetToNearestZone( Camera.main.GetComponent<Transform>().position );
			EditorUtility.SetDirty( RenderSettingsManager.instance );
		}

		EditorGUILayout.Space();

		_renderSettingsZone = (RenderSettingsZone)EditorGUILayout.ObjectField( "Render zone",
		                                                                       _renderSettingsZone,
		                                                                       typeof( RenderSettingsZone ),
		                                                                       true );

		if( _renderSettingsZone )
		{
			if( GUILayout.Button( "Display selected render volume" ) )
			{
				RenderSettingsManager.SetRenderSettings( _renderSettingsZone.lightingSettings );
				EditorUtility.SetDirty( RenderSettingsManager.instance );
			}
		}

		EditorGUILayout.Space();

		_dayLightingSettings = (TimeLightingSettingsData)EditorGUILayout.ObjectField( "Day lighting settings",
		                                                                              _dayLightingSettings,
		                                                                              typeof( TimeLightingSettingsData ),
		                                                                              false );

		_nightLightingSettings = (TimeLightingSettingsData)EditorGUILayout.ObjectField( "Night lighting settings",
		                                                                                _nightLightingSettings,
		                                                                                typeof( TimeLightingSettingsData ),
		                                                                                false );

		if( _dayLightingSettings != null && _nightLightingSettings != null )
		{
			if( GUILayout.Button( "Display selected time lighting settings" ) )
			{
				RenderSettingsManager.SetRenderSettings( new LightingSettings( _dayLightingSettings.timeSettings, _nightLightingSettings.timeSettings ) );
				EditorUtility.SetDirty( RenderSettingsManager.instance );
			}
		}
	}
}