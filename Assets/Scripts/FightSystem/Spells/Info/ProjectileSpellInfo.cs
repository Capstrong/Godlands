using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProjectileSpellInfo : SpellInfo
{
	public float moveSpeed;

	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Spells/Projectile")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<ProjectileSpellInfo> ();
	}
	#endif
}
