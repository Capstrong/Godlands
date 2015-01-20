using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpellChargeAttack : SpellAttack 
{
	public SpellAttackEvent endSpellEvent;
	public float maxChargeTime = 5f;

	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Attacks/Spell Attacks/Charge Spell")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<SpellChargeAttack> ();
	}
	#endif
}
