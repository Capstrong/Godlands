using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ResourceData : ScriptableObject 
{
	public Sprite icon;
	public GameObject prefab;
	// public StatEffect[] statEffects

	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Resource Data")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<ResourceData> ();
	}
	#endif
}
