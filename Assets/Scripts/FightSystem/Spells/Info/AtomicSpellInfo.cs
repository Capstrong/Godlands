using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AtomicSpellInfo : SpellInfo
{
	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Spells/Atomic")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<AtomicSpellInfo> ();
	}
	#endif
}
