using UnityEngine;
using System.Collections;

public class SkillUI : MonoBehaviour
{
	[SerializeField] Stat _stat;

	protected StatUIIconTag _icon = null;
	protected PlayerStats _playerStats = null;

	public virtual void SetStat( float stat ) { } 
	public virtual void SetMaxStat( float stat ) { }

	protected virtual void Awake()
	{
		_icon = GetComponentInChildren<StatUIIconTag>();
		_playerStats = GameObject.FindObjectOfType<PlayerStats>();

		_playerStats.GetStatObject( _stat ).UpdateStatCallback += SetStat;
		_playerStats.GetStatObject( _stat ).UpdateMaxStatCallback += SetMaxStat;
	}
}
