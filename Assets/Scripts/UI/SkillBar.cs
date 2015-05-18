using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillBar : SkillUI 
{
	[SerializeField] float _statScaleMod = 200f;

	RectTransform _uiBarRectFront = null;
	RectTransform _uiBarRectBack = null;

	protected override void Awake()
	{
		base.Awake();
		_uiBarRectBack = GetComponent<RectTransform>();
		_uiBarRectFront = GetComponent<Transform>().GetChild( 0 ).GetComponent<RectTransform>();

		SetStat( 0f );
		SetMaxStat( 0f );
	}

	public override void SetStat( float stat )
	{
		_uiBarRectFront.sizeDelta = new Vector2( stat * _statScaleMod, _uiBarRectFront.sizeDelta.y );
	}

	public override void SetMaxStat( float stat )
	{
		if ( stat > 0f )
		{
			_icon.gameObject.SetActive( true );
		}
		else
		{
			_icon.gameObject.SetActive( false );
		}

		_uiBarRectBack.sizeDelta = new Vector2( stat * _statScaleMod, _uiBarRectBack.sizeDelta.y );
	}
}
