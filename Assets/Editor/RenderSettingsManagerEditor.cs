using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RenderSettingsManager))]
public class RenderSettingsManagerEditor : Editor 
{
	RenderSettingsZone _renderSettingsZone = null;
	TimeLightingSettingsData _dayLightingSettings = null;
	TimeLightingSettingsData _nightLightingSettings = null;

	bool _updateWithNearestRenderZone = false;

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		_updateWithNearestRenderZone = GUILayout.Toggle( _updateWithNearestRenderZone, "Update With Nearest Render Zone" );

		if( !_updateWithNearestRenderZone )
		{
			if( GUILayout.Button( "Display nearest render volume" ) )
			{
				RenderSettingsManager.SetToNearestZone( Camera.main.GetComponent<Transform>().position );
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
				}
			}
		}
		else
		{
			var renderSettingsManager = target as RenderSettingsManager;
			EditorUtility.SetDirty( renderSettingsManager );
			RenderSettingsManager.SetToNearestZone( Camera.main.GetComponent<Transform>().position );
		}
	}
}