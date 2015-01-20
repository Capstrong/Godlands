using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RangedAttack : Attack 
{
	[SerializeField] float moveSpeed = 15f;

	float chargeAmount = 0f;

	void Awake()
	{
	}

	#if UNITY_EDITOR
	[MenuItem("Assets/Create/RangedAttack")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<RangedAttack> ();
	}
	#endif
}
