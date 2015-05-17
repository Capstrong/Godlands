using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour 
{
	[SerializeField] Stat _stat = 0;
	[SerializeField] float _statScaleMod = 200f;

	RectTransform _uiBarRectFront = null;
	RectTransform _uiBarRectBack = null;
	StatUIIconTag _icon = null;

	PlayerStats _playerStats = null;

	void Awake()
	{
		_uiBarRectBack = GetComponent<RectTransform>();
		_uiBarRectFront = GetComponent<Transform>().GetChild( 0 ).GetComponent<RectTransform>();
		_icon = GetComponentInChildren<StatUIIconTag>();

		_playerStats = GameObject.FindObjectOfType<PlayerStats>();
	}

	void Update()
	{
		float statValue = _playerStats.GetStatValue( _stat );
		float maxStatValue = _playerStats.GetStatMaxValue( _stat );

		_uiBarRectFront.sizeDelta = new Vector2( _playerStats.GetStatValue( _stat ) * _statScaleMod, _uiBarRectFront.sizeDelta.y );
		_uiBarRectBack.sizeDelta = new Vector2( _playerStats.GetStatMaxValue( _stat ) * _statScaleMod, _uiBarRectBack.sizeDelta.y );

		if ( maxStatValue > 0 )
		{
			_icon.gameObject.SetActive( true );
		}
		else
		{
			_icon.gameObject.SetActive( false );
		}
	}
}
