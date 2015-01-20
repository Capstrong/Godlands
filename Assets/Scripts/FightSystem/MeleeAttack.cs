using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum MeleeType
{
	Light,
	Heavy
}

[System.Serializable]
public class NextHit
{
	public MeleeType hitType;
	public MeleeAttack attack;
	public FrameWindow frameWindow;
}

[System.Serializable]
public class CollisionInfo
{
	public BoneLocation[] boneLocations;
	public float colliderScale = 1f;

	public FrameWindow frameWindow;
}

public class MeleeAttack : Attack 
{
	public ColliderType colliderType = ColliderType.Sphere;
	public DamageType damageType = DamageType.Normal;
	
	public int baseDamage = 35;
	public int intensityRating = 1;
	public int defensePenetration = 0;

	public float lifetime
	{
		get { return attackAnim.length + vulnerableTime; }
	}

	public NextHit[] possibleNextHits;
	public CollisionInfo[] collisionInfos;

	void Awake()
	{

	}

	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Attacks/Melee Attack")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<MeleeAttack> ();
	}
	#endif
}
