using UnityEngine;
using System.Collections;

public class SkillUI : MonoBehaviour
{
	[SerializeField] 
	protected Stat _stat = Stat.Invalid;
	protected StatUIIconTag _icon = null;
	protected PlayerStats _playerStats = null;

	public virtual void SetStat( float stat ) { } 
	public virtual void SetMaxStat( float stat ) { }

	public SkillUI() { }

	protected void Awake()
	{
		_icon = GetComponentInChildren<StatUIIconTag>();
		_playerStats = GameObject.FindObjectOfType<PlayerStats>();
	}
}
