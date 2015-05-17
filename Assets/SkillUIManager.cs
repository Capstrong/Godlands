using UnityEngine;
using System.Collections;

public class SkillUIManager : SingletonBehaviour<SkillUIManager>
{
	[System.Serializable]
	class SkillUIDictionary : SerializableDictionary<Stat, SkillUI> { }

	[SerializeField] SkillUIDictionary _skillUIDictionary = null;

	public static void UpdateStatUI( Stat stat, float statValue )
	{
		instance._skillUIDictionary[stat].SetStat( statValue );
	}

	public static void UpdateMaxStatUI( Stat stat, float maxStatValue )
	{
		instance._skillUIDictionary[stat].SetMaxStat( maxStatValue );
	}
}
