using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour 
{
	protected Actor caster;
	protected SpellInfo spellInfo;
	float lifeTimer = 0f;

	public virtual void Init(Actor inCaster, Vector3 inVelocity, SpellInfo inSpellInfo)
	{
		caster = inCaster;
		spellInfo = inSpellInfo;
		rigidbody.velocity = inVelocity;
	}
}
