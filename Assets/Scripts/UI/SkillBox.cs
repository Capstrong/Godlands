using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillBox : SkillUI 
{
	Text[] _textObjs;
	Image _statImage = null;

	Color _initColor = Color.white;

	[SerializeField] float _statScaleMod = 3f;

	new void Awake()
	{
		base.Awake();

		_textObjs = GetComponentsInChildren<Text>();		

		_statImage = GetComponent<Image>();
		_initColor = _statImage.color;

		SetStat( 0f );
		SetMaxStat( 0f );
	}

	public override void SetStat( float stat )
	{
		int currentStatAmount = Mathf.CeilToInt( stat * _statScaleMod );

		if ( currentStatAmount == 0 )
		{
			HideStatBox();
		}
		else
		{
			_icon.gameObject.SetActive( true );

			foreach( Text text in _textObjs )
			{
				text.text = currentStatAmount.ToString();
			}

			_statImage.color = _initColor;
		}
	}

	public override void SetMaxStat( float stat ) { }

	void HideStatBox()
	{
		foreach( Text text in _textObjs )
		{
			text.text = "";
		}
		
		_icon.gameObject.SetActive( false );

		_statImage.color = _statImage.color.SetAlpha( 0f );
	}
}
