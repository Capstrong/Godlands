using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AreaSpellInfo : SpellInfo
{
	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Spells/Area")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<AreaSpellInfo> ();
	}
	#endif
}
