using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillBubbles : SkillUI
{
	[Tooltip( "Bubble number = stat * statScale + statIntercept" )]
	[SerializeField] float _statScale = 3f;
	[Tooltip( "Bubble number = stat * statScale + statIntercept" )]
	[SerializeField] float _statIntercept = 0f;
	[SerializeField] Image[] _bubbles = null;

	Color _fullColor = Color.white;

	static Color _blank = new Color( 0f, 0f, 0f, 0f );

	public new void Awake()
	{
		base.Awake();

		_fullColor = _bubbles[0].color;

		foreach ( Image bubbleImage in _bubbles )
		{
			bubbleImage.color = _blank;
		}

		SetMaxStat( 0f );
	}

	public override void SetMaxStat( float stat )
	{
		if ( stat == 0 )
		{
			_icon.gameObject.SetActive( false );
		}
		else
		{
			_icon.gameObject.SetActive( true );
		}

		float fullStat = stat * _statScale + _statIntercept;
		int flooredStat = Mathf.FloorToInt( fullStat );

		for ( int i = 0; i <= flooredStat && i < _bubbles.Length; i++ )
		{
			_bubbles[i].color = _fullColor;
		}

		for ( int i = flooredStat + 1; i < _bubbles.Length; i++ )
		{
			_bubbles[i].color = _blank;
		}

		if ( flooredStat >= 0 && flooredStat < _bubbles.Length )
		{
			_bubbles[flooredStat].color = _bubbles[flooredStat].color.SetAlpha( fullStat - flooredStat );
		}
	}
}
