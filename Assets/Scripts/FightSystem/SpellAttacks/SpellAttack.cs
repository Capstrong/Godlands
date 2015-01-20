using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SpellAttackEvent
{
	public SpellInfo[] spells;
	public AnimationClip castAnim;
}

[System.Serializable]
public class SpellAttackFrameEvent
{
	public SpellInfo[] spells;
	public AnimationClip castAnim;
	public int frame;
	public bool usedThisLoop = false;
}

public class SpellAttack : Attack 
{
	public SpellAttackFrameEvent[] routineFrameEvents;
	public float routineLength = -1f; // use this if routine is longer than attack anim length
}
