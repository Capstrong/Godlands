using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Stat
{
	Invalid,
	Stamina,
	Gliding,
	Cutting,
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
public class StatDictionary : SerializableDictionary<Stat, StatObject> { } // Necessary for serialization

[RequireComponent( typeof(PlayerActorPhysics) )]
public class PlayerActorStats : ActorComponent 
{
	[SerializeField] StatDictionary _statDictionary = new StatDictionary();

	public override void Awake()
	{
		base.Awake();

		foreach (KeyValuePair<Stat,StatObject> pair in _statDictionary)
		{
			pair.Value.currentMax = pair.Value.startingMax;
			pair.Value.currentValue = pair.Value.startingMax;
			ScaleMaxImage( pair.Value );
		}
	}

	public void IncrementMaxStat( Stat stat )
	{
		StatObject statObject = _statDictionary[stat];
		statObject.currentMax += statObject.maxIncrement;
		ScaleCurrImage( statObject );
	}

	public void DecrementMaxStat( Stat stat )
	{
		StatObject statObject = _statDictionary[stat];
		statObject.currentMax = Mathf.Max(statObject.currentMax - statObject.maxIncrement, 0.0f); // decrement and clamp at a minimum of 0
		ScaleMaxImage( statObject );
	}

	public bool CanUseStat( Stat stat )
	{
		return (_statDictionary[stat].currentValue > 0.0f);
	}

	public void StartUsingStat( Stat stat )
	{
		_statDictionary[stat].isUsing = true;
	}

	public void StopUsingStat( Stat stat )
	{
		_statDictionary[stat].isUsing = false;
	}

	public float GetStatValue( Stat stat )
	{
		return _statDictionary[stat].currentValue;
	}

	void Update()
	{
		foreach ( KeyValuePair<Stat, StatObject> pair in _statDictionary )
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
					statObject.currentValue += statObject.rechargeRate;

					if ( statObject.currentValue > statObject.currentMax )
					{
						statObject.currentValue = statObject.currentMax;
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
