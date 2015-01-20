using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpellChannelAttack : SpellAttack 
{
	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Attacks/Spell Attacks/Channel Spell")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<SpellChannelAttack> ();
	}
	#endif
}
