using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillBox : MonoBehaviour 
{
	[SerializeField] Stat _stat = 0;	
	PlayerStats _playerStats = null;

	Text[] _textObjs;
	Image _statImage = null;

	Color _initColor = Color.white;

	[SerializeField] float _statScaleMod = 3f;

	int _prevStatAmount = 0;
	int _currentStatAmount = 0;

	void Awake()
	{
		_playerStats = GameObject.FindObjectOfType<PlayerStats>();
		_textObjs = GetComponentsInChildren<Text>();

		_statImage = GetComponent<Image>();
		_initColor = _statImage.color;

		if( _currentStatAmount == 0 )
		{
			HideStatBox();
		}
	}

	void Update()
	{
		_currentStatAmount = Mathf.CeilToInt( _playerStats.GetStatValue( _stat ) * _statScaleMod );

		if( _currentStatAmount != _prevStatAmount )
		{
			if( _currentStatAmount == 0 )
			{
				HideStatBox();
			}
			else
			{
				foreach( Text text in _textObjs )
				{
					text.text = _currentStatAmount.ToString();
				}

				_statImage.color = _initColor;
			}
		}
	}

	void HideStatBox()
	{
		foreach( Text text in _textObjs )
		{
			text.text = "";
		}
		
		_statImage.color = Color.white - new Color( 0f, 0f, 0f, 1f );
	}
}
