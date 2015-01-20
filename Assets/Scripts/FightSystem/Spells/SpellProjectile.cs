using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpellProjectile : Spell
{
	public float moveSpeed;

	public override void Init(Actor inCaster, Vector3 inVelocity, SpellInfo inSpellInfo)
	{
		caster = inCaster;
		spellInfo = inSpellInfo;

		float rotOffset = inCaster.GetCamera().transform.eulerAngles.y - inCaster.GetModel().transform.eulerAngles.y;
		Vector3 outDir = Quaternion.Euler(0f, rotOffset, 0f) * inCaster.GetCamera().transform.forward;

		rigidbody.velocity = inVelocity + outDir * moveSpeed;
	}
}
