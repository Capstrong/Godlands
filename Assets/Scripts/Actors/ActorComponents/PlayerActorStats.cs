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

	public void IncrementMaxStat( Stats stat )
	{
		StatObject statObject = statDictionary[stat];
		statObject.currentMax += statObject.maxIncrement;
		ScaleCurrImage( statObject );
	}

	public void DecrementMaxStat( Stats stat )
	{
		StatObject statObject = statDictionary[stat];
		statObject.currentMax = Mathf.Max(statObject.currentMax - statObject.maxIncrement, 0.0f); // decrement and clamp at a minimum of 0
		ScaleMaxImage( statObject );
	}

	public bool CanUseStat( Stats stat )
	{
		return (statDictionary[stat].currentValue > 0.0f);
	}

	public void StartUsingStat( Stats stat )
	{
		statDictionary[stat].isUsing = true;
	}

	public void StopUsingStat( Stats stat )
	{
		statDictionary[stat].isUsing = false;
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

					StopUsingStat( pair.Key );
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
