using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour 
{
	[SerializeField] Stat _stat = 0;
	[SerializeField] float _statScaleMod = 200f;

	RectTransform _uiBarRectFront = null;
	RectTransform _uiBarRectBack = null;

	PlayerStats _playerStats = null;

	void Awake()
	{
		_uiBarRectBack = GetComponent<RectTransform>();
		_uiBarRectFront = GetComponent<Transform>().GetChild(0).GetComponent<RectTransform>();

		_playerStats = GameObject.FindObjectOfType<PlayerStats>();
	}

	void Update()
	{
		_uiBarRectFront.sizeDelta = new Vector2( _playerStats.GetStatValue( _stat ) * _statScaleMod, _uiBarRectFront.sizeDelta.y );
		_uiBarRectBack.sizeDelta = new Vector2( _playerStats.GetStatMaxValue( _stat ) * _statScaleMod, _uiBarRectBack.sizeDelta.y );
	}
}
