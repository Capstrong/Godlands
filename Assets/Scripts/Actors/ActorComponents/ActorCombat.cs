using UnityEngine;
using System.Collections;

public class ActorCombat : ActorComponent 
{
	[SerializeField] float attackCooldownTime = 0f;
	float attackCooldownTimer = 0f;

	GameObject attackCollider;

	bool melee1Input = false;
	bool melee2Input = false;

	bool spellInput = false;
	float spellInputTimer = 0f;

	bool chanellingSpell = false;

	bool incorrectInput = false;

	[SerializeField] MeleeAttack lightAttack;
	[SerializeField] MeleeAttack heavyAttack;

	[SerializeField] float inputCooldownTime = 0.1f;
	float inputCooldownTimer = 1f;

	[SerializeField] SpellAttack[] spellAttacks;
	SpellAttack currentSpellAttack;

	Attack currentAttack;

	delegate IEnumerator AttackRoutine(); // so I can pass routines as arguments

	void Awake()
	{
		attackCollider = new GameObject("AttackCollider");
		attackCollider.transform.parent = transform;

		//currentSpellAttack = spellAttacks[0];

		foreach(SpellAttack spellAttack in spellAttacks)
		{
			ResetSpellAttackRoutines(spellAttack);
		}
	}

	void Update()
	{
		melee1Input = Input.GetButtonDown("Melee1" + WadeUtils.platformName);
		melee2Input = Input.GetButtonDown("Melee2" + WadeUtils.platformName);
		
		spellInput = Input.GetButton("Spell" + WadeUtils.platformName);

		// Check update here instead of fixed so we don't miss any presses
		if(actor.GetPhysics().CanAttack())
		{
			MeleeCheck();
			SpellCheck();
			
			if(attackCooldownTimer >= attackCooldownTime)
			{
				if(!chanellingSpell)
				{
					currentAttack = null;
					DisableAnimatorIsAttacking();
				}

				incorrectInput = false;
			}
		}

		attackCooldownTimer += Time.deltaTime;
		inputCooldownTimer += Time.deltaTime;
	}

	void MeleeCheck()
	{
		if(inputCooldownTimer > inputCooldownTime && !incorrectInput && !chanellingSpell)
		{
			if((melee1Input || melee2Input))
			{
				if(currentAttack)
				{
					MeleeType meleeType = melee1Input ? MeleeType.Light : MeleeType.Heavy;
					MeleeAttack nextAttack = GetNextAttack((MeleeAttack)currentAttack, meleeType);
					if(nextAttack)
					{
						currentAttack = nextAttack;
						StopAllCoroutines();
						StartMelee();
						return;
					}
				}
				else if(lightAttack && heavyAttack)
				{
					currentAttack = melee1Input ? lightAttack : heavyAttack;
					StartMelee();
					return;
				}
				
				incorrectInput = true;
			}
		}
	}

	void StartMelee()
	{
		StartCoroutine(DoMeleeAttack());
		inputCooldownTimer = 0f;
	}
	
	MeleeAttack GetNextAttack(MeleeAttack meleeAttack, MeleeType meleeType)
	{
		if(meleeAttack.possibleNextHits.Length > 0)
		{
			foreach(NextHit nextHit in meleeAttack.possibleNextHits)
			{
				if(nextHit.hitType == meleeType && 
				   nextHit.frameWindow.CheckInBounds(attackCooldownTimer * 1f/Time.deltaTime))
				{
					return nextHit.attack;
				}
			}
		}
		
		return null;
	}
	
	IEnumerator DoMeleeAttack()
	{
		MeleeAttack meleeAttack = currentAttack as MeleeAttack;
		
		attackCooldownTime = meleeAttack.lifetime;
		EnableAnimatorIsAttacking ();
		
		actor.GetPhysics().moveSpeedMod = meleeAttack.playerSpeedMod;
		
		actor.GetAnimator().StopPlayback();
		actor.GetAnimator().Play(meleeAttack.attackAnim.name, 0, 0f);
		
		attackCooldownTimer = 0f;
		
		while(attackCooldownTimer < attackCooldownTime)
		{
			foreach(CollisionInfo colInfo in meleeAttack.collisionInfos)
			{
				if(colInfo.frameWindow.IsStartTime(attackCooldownTimer * 60f))
				{
					//TODO: this should be converted to draw from a pool of colliders/gos
					foreach(BoneLocation boneLocation in colInfo.boneLocations)
					{
						GameObject hitboxObj = new GameObject("Hitbox", typeof(Rigidbody));
						Destroy(hitboxObj, colInfo.frameWindow.Length * Time.deltaTime);
						
						hitboxObj.layer = gameObject.layer;
						hitboxObj.rigidbody.isKinematic = true;
						
						AttackHitbox hitbox = hitboxObj.AddComponent<AttackHitbox>();
						hitbox.attacker = this;
						hitbox.attack = meleeAttack;
						hitbox.enabled = true;
						
						if(meleeAttack.colliderType == ColliderType.Box)
						{
							BoxCollider boxCol = hitboxObj.AddComponent<BoxCollider>();
							boxCol.size = Vector3.one * colInfo.colliderScale;
						}
						else
						{
							SphereCollider sphereCol = hitboxObj.AddComponent<SphereCollider>();
							sphereCol.radius = colInfo.colliderScale;
						}
						
						hitboxObj.collider.isTrigger = true;
						
						hitboxObj.transform.parent = actor.GetBoneAtLocation(boneLocation);
						hitboxObj.transform.localPosition = Vector3.zero;
						hitboxObj.transform.localRotation = Quaternion.identity;
					}
				}
			}
			
			yield return 0;
		}
		
		actor.GetPhysics().moveSpeedMod = 1f;
		DisableAnimatorIsAttacking ();
	}

	void SpellCheck()
	{
		float currentFrame = spellInputTimer * 1f/Time.deltaTime;

		if(spellInput)
		{
			if(!chanellingSpell)
			{
				if(attackCooldownTimer >= attackCooldownTime)
				{
					StartSpellAttack();
				}
				else
				{
					return;
				}
			}

			SpellAttack spellAttack = currentAttack as SpellAttack;
			currentFrame = spellInputTimer * 1f/Time.deltaTime;

			if(spellAttack.routineFrameEvents.Length > 0)
			{
				foreach(SpellAttackFrameEvent frameEvent in spellAttack.routineFrameEvents)
				{
					if(!frameEvent.usedThisLoop && WadeUtils.IsWithinFrame(currentFrame, frameEvent.frame))
					{
						frameEvent.usedThisLoop = true;

						if(!actor.GetAnimator().GetBool("isCastingSpellEvent"))
						{
							foreach(SpellInfo spell in frameEvent.spells)
							{
								DoSpell(spell);
							}

							if(frameEvent.castAnim)
							{
								actor.GetAnimator().StopPlayback();
								actor.GetAnimator().Play(frameEvent.castAnim.name, 0, 0f);

								actor.GetAnimator().SetBool("isCastingSpellEvent", true);
								Invoke("DisableAnimatorCastingSpellEvent", frameEvent.castAnim.length);
							}
						}
						else
						{
							Debug.LogError("Another spell is currently active! (Frame: " + currentFrame + ", Spell: " + spellAttack.name + ")");
						}
					}
				}
			}

			spellInputTimer += Time.deltaTime;

			float currentAnimLength = spellAttack.routineLength > 0f ? 	spellAttack.routineLength : currentAttack.attackAnim.length;
			currentAnimLength *= 1f/Time.deltaTime;

			// Loop input timer if above anim length
			if(currentFrame > currentAnimLength)
			{
				spellInputTimer = 0f + (currentFrame - currentAnimLength) * Time.deltaTime;
				ResetSpellAttackRoutines(currentAttack as SpellAttack);
			}
		}
		else if(chanellingSpell)
		{
			ResetSpellAttackRoutines(currentAttack as SpellAttack);
			FinishSpellAttack();
		}
	}

	void ResetSpellAttackRoutines(SpellAttack spellAttack)
	{
		foreach(SpellAttackFrameEvent frameEvent in spellAttack.routineFrameEvents)
		{
			frameEvent.usedThisLoop = false;
		}
	}

	void StartSpellAttack()
	{
		currentAttack = currentSpellAttack;
		chanellingSpell = true;
		spellInputTimer = 0f;

		actor.GetPhysics().moveSpeedMod = currentAttack.playerSpeedMod;

		if(currentAttack.attackAnim)
		{
			actor.GetAnimator().StopPlayback();
			actor.GetAnimator().Play(currentAttack.attackAnim.name, 0, 0f);
		}
		
		EnableAnimatorIsAttacking ();
	}
	
	void FinishSpellAttack()
	{
		chanellingSpell = false;

		attackCooldownTime = currentAttack.vulnerableTime;
		attackCooldownTimer = 0f;

		if( currentAttack is SpellChargeAttack )
		{
			StartCoroutine( QueueSpellEvent( currentAttack as SpellChargeAttack ) );
		}
		else
		{
			EndCastChores();
		}
	}

	void EnableAnimatorIsAttacking()
	{
		if ( actor.GetAnimator() ) actor.GetAnimator().SetBool( "isAttacking", true );
	}

	void DisableAnimatorIsAttacking()
	{
		if ( actor.GetAnimator() ) actor.GetAnimator().SetBool( "isAttacking", false );
	}

	void DisableAnimatorCastingSpellEvent()
	{
		if ( actor.GetAnimator() ) actor.GetAnimator().SetBool( "isCastingSpellEvent", false );
	}

	IEnumerator QueueSpellEvent( SpellChargeAttack chargeSpell )
	{
		while(actor.GetAnimator().GetBool("isCastingSpellEvent") && attackCooldownTimer < attackCooldownTime)
		{
			yield return 0;
		}

		if(chargeSpell.endSpellEvent.castAnim && attackCooldownTime > 0f)
		{
			actor.GetAnimator().StopPlayback();
			actor.GetAnimator().Play(chargeSpell.endSpellEvent.castAnim.name, 0, 0f);

			Invoke("DisableAnimatorIsAttacking", chargeSpell.endSpellEvent.castAnim.length);
			Invoke("EndCastChores", chargeSpell.endSpellEvent.castAnim.length);
		}
		else
		{
			DisableAnimatorIsAttacking();
			EndCastChores();
		}

		foreach(SpellInfo spell in chargeSpell.endSpellEvent.spells)
		{
			DoSpell (spell);
		}
	}

	void EndCastChores()
	{
		actor.GetPhysics().moveSpeedMod = 1f;
	}

	void DoSpell(SpellInfo spellInfo)
	{
		GameObject spellObj = WadeUtils.Instantiate (spellInfo.vfx);

		if(spellInfo.spawnOnBone)
		{
			Transform spawnBone = actor.GetBoneAtLocation (spellInfo.spawnBone);
			spellObj.transform.position = spawnBone.transform.position;

			if(spellInfo.parentToBone)
			{
				spellObj.transform.parent = spawnBone;
				spellObj.transform.localPosition = spellInfo.spawnOffset;
				spellObj.transform.localRotation = Quaternion.identity;
			}
		}
		else
		{
			spellObj.transform.position = transform.position + spellInfo.spawnOffset;
		}

		spellObj.GetComponent<Spell> ().Init (actor, rigidbody.velocity, spellInfo);
	}

	public void OnLandedAttack(Attack attack, Actor actor)
	{

	}

	public void OnAttacked(Attack attack)
	{
		actor.OnAttacked(attack);
	}
}
