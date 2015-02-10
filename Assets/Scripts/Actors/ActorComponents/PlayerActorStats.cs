using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Stats
{
	Invalid,
	Stamina,
	Gliding
}

[System.Serializable]
public class StatObject
{
	public float startingMax = 0.0f;
	public float maxIncrement = 0.0f;
	public float useRate = 0.0f; // Stat units per second
	public float rechargeRate = 0.0f;
	public float currentMax = 0.0f;
	public float currentValue = 0.0f;
	public bool isUsing = false;
	public Image currentImage = null;
	public Image maxImage = null;
	public float statToScaleRatio = 0.0f;
}

[System.Serializable]
public class StatDictionary : SerializableDictionary<Stats, StatObject> { } // Necessary for serialization

[RequireComponent( typeof(PlayerActorPhysics) )]
public class PlayerActorStats : ActorComponent 
{
	[SerializeField] StatDictionary statDictionary = new StatDictionary();

	PlayerActorPhysics _actorPhysics = null;

	public override void Awake()
	{
		base.Awake();
		_actorPhysics = GetComponent<PlayerActorPhysics>();

		foreach (KeyValuePair<Stats,StatObject> pair in statDictionary)
		{
			pair.Value.currentMax = pair.Value.startingMax;
			pair.Value.currentValue = pair.Value.startingMax;
			ScaleMaxImage( pair.Value );
		}
	}

	public void IncrementMaxStamina()
	{
		StatObject staminaObject = statDictionary[Stats.Stamina];
		staminaObject.currentMax += staminaObject.maxIncrement;
		ScaleCurrImage( staminaObject );
	}

	public void DecrementMaxStamina()
	{
		StatObject staminaObject = statDictionary[Stats.Stamina];
		staminaObject.currentMax = Mathf.Max(staminaObject.currentMax - staminaObject.maxIncrement, 0.0f); // decrement and clamp at a minimum of 0
		ScaleMaxImage( staminaObject );
	}

	public bool CanUseStamina()
	{
		return (statDictionary[Stats.Stamina].currentValue > 0.0f);
	}

	public void StartUsingStamina()
	{
		statDictionary[Stats.Stamina].isUsing = true;
	}

	public void StopUsingStamina()
	{
		statDictionary[Stats.Stamina].isUsing = false;
	}

	void Update()
	{
		foreach ( KeyValuePair<Stats, StatObject> pair in statDictionary )
		{
			StatObject statObject = pair.Value;

			if ( statObject.isUsing )
			{
				statObject.currentValue -= statObject.useRate;

				if ( statObject.currentValue <= 0 )
				{
					statObject.currentValue = 0;

					StopUsingStamina();
				}
			}
			else
			{
				if ( !_actorPhysics.isGrabbing )
				{
					statObject.currentValue += statObject.rechargeRate;

					if ( statObject.currentValue > statObject.currentMax )
					{
						statObject.currentValue = statObject.currentMax;
					}
				}
			}

			ScaleCurrImage( statObject );
		}
	}

	void ScaleMaxImage( StatObject statObject )
	{
		float scale = statObject.currentMax * statObject.statToScaleRatio;
		// TODO: cache off transform to save on getComponent() calls
		statObject.maxImage.transform.SetScale( scale, scale, scale );
	}

	void ScaleCurrImage( StatObject statObject )
	{
		float scale = statObject.currentValue * statObject.statToScaleRatio;
		// TODO: cache off transform to save on getComponent() calls
		statObject.currentImage.transform.SetScale( scale, scale, scale );
	}
}
