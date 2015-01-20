using UnityEngine;
using System.Collections;

public class SpellInfo : ScriptableObject 
{
	public GameObject vfx;
	public int baseDamage = 10;
	public int chargeAmount = 0;

	public BoneLocation spawnBone;
	public bool spawnOnBone = false;
	public bool parentToBone = false;
	public Vector3 spawnOffset;

	public float lifetime = 1f;

	public virtual void Init(Actor actor, Vector3 initVelocity)
	{

	}
}
