using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackHitbox : MonoBehaviour 
{
	public ActorCombat attacker;
	public Attack attack;
	public List<Collider> hitColliders = new List<Collider>();

	void Update()
	{
	}

	void OnTriggerEnter(Collider col)
	{
		if(this.enabled && col.transform != transform && !hitColliders.Contains(col))
		{
			ActorCombat[] actorCombats = col.transform.root.GetComponents<ActorCombat>();

			if(actorCombats.Length > 0)
			{
				foreach(ActorCombat actorCombat in actorCombats)
				{
					if( actorCombat != null )
					{
						attacker.OnLandedAttack(attack, actorCombat.actor);
						actorCombat.OnAttacked(attack);
					}
				}

				hitColliders.Add(col);
			}
		}
	}
}
