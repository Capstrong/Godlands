using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TravelSoundPlayer : ScriptableObject
{
	[SerializeField] AudioClip[] _travelSounds;
	public AudioClip[] travelSounds
	{
		get { return _travelSounds; }
	}

	public AudioClip GetRandomClip()
	{
		return _travelSounds[Random.Range( 0, _travelSounds.Length )];
	}

#if UNITY_EDITOR
	[MenuItem( "Assets/Create/Travel Sound Player" )]
	public static void CreateTravelSoundPlayerAsset()
	{
		// Create asset.
		TravelSoundPlayer travelSoundPlayer = ScriptableObject.CreateInstance<TravelSoundPlayer>();
		AssetDatabase.CreateAsset( travelSoundPlayer, "Assets/NewTravelSoundPlayer.asset" );

		// Set asset as the active object.
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = travelSoundPlayer;
	}
#endif
}
