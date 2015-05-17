using UnityEngine;
using System.Collections;

public class SkillUIManager : SingletonBehaviour<SkillUIManager>
{
	[System.Serializable]
	class SkillUIDictionary : SerializableDictionary<Stat, SkillUI> { }

	[SerializeField] SkillUIDictionary _skillUIDictionary = null;

	public void UpdateStatUI( Stat stat, float statValue )
	{
		_skillUIDictionary[stat].SetStat( statValue );
	}

	public void UpdateMaxStatUI( Stat stat, float maxStatValue )
	{
		_skillUIDictionary[stat].SetMaxStat( maxStatValue );
	}
}
