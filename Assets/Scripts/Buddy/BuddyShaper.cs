using UnityEngine;
using System.Collections;

[System.Serializable]
public struct BuddyTypeFashion
{
	public Color[] topBodyColors;
	public Color[] bottomBodyColors;
}

public class BuddyShaper : MonoBehaviour
{
	[Tooltip( "Need reference to buddy stats to choose clothing colors." )]
	[SerializeField] BuddyStats _buddyStats = null;

	// Blend Shapes
	public SkinnedMeshRenderer skinnedMeshRend = null;
	[SerializeField] int _blendShapeCount = 0;

	[Tooltip( "Index of leg length Blendshape" )]
	[SerializeField] int _legIndex = 0;
	float _legHeightOffsetMod = 0.0006f; // Offset used to reposition mesh so feet are always touching the floor

	[Tooltip( "Indices of Blendshapes which can overlap (think nose and head shapes)." )]
	[SerializeField] int[] _overlapBlendIndices = null;

	[Tooltip( "Indices of Blendshapes which cannot overlap (think different hat/hood types)." )]
	[SerializeField] int[] _exclusiveBlendIndices = null;
	[SerializeField] MinMaxF _exclusiveBlendRange = new MinMaxF( 50f, 100f );

	// Scaling
	[Tooltip( "Min and max scale amount." )]
	[SerializeField] MinMaxF _heightScaleRange = new MinMaxF( 0.8f, 1.2f );

	// Coloring
	[Tooltip( "Buddy clothing color sets." )]
	[SerializeField] BuddyTypeFashion[] _buddyTypeFashions = null;
	[SerializeField] Texture2D[] _skinColors = null;

	Vector3 _initPos = Vector3.zero;
	Vector3 _initScale = Vector3.one;

	bool _prevDebugRandomizer = false;
	[SerializeField] bool _debugRandomizer = false;

	void Start()
	{
		skinnedMeshRend = GetComponentInChildren<SkinnedMeshRenderer>();

		_initPos = transform.position;
		_initScale = transform.localScale;

		if( _debugRandomizer )
		{
			InvokeRepeating( "RandomizeBuddy", 0.3f, 0.3f );
		}
		else
		{
			RandomizeBuddy();
		}
	}

	void Update()
	{
		if( _debugRandomizer != _prevDebugRandomizer )
		{
			if( _debugRandomizer )
			{
				InvokeRepeating( "RandomizeBuddy", 0.3f, 0.3f );
			}
			else
			{
				CancelInvoke( "RandomizeBuddy");
			}

			_prevDebugRandomizer = _debugRandomizer;
		}
	}

	void RandomizeBuddy()
	{
		for( int i = 0; i < _blendShapeCount; i++ )
		{
			skinnedMeshRend.SetBlendShapeWeight( i, 0f );
		}

		AdjustBlendShapes();
		AdjustSize();
		AdjustColor();
	}

	void AdjustBlendShapes()
	{
		if( _blendShapeCount < 1 )
		{
			Debug.LogError( "No blendshapes found. Check model." );
		}

		foreach( int i in _overlapBlendIndices )
		{
			float setWeight = Random.Range( 0f, 100f );
			skinnedMeshRend.SetBlendShapeWeight( i, setWeight );

			if( i == _legIndex )
			{
				transform.position = _initPos + Vector3.up * setWeight * _legHeightOffsetMod;
			}
		}

		int exclusiveBlendIndex = Random.Range( 0, _exclusiveBlendIndices.Length );
		skinnedMeshRend.SetBlendShapeWeight( 	_exclusiveBlendIndices[exclusiveBlendIndex],
		                                    	Random.Range( _exclusiveBlendRange.min, _exclusiveBlendRange.max ) );
	}

	void AdjustSize()
	{
		Vector3 adjustedScale = _initScale;
		adjustedScale.y *= _heightScaleRange.random;

		transform.localScale = adjustedScale;
	}

	void AdjustColor()
	{
		if( _buddyStats.itemData )
		{
			int statNum = (int)_buddyStats.itemData.stat;
			int topBodyColorIndex = Random.Range( 0, _buddyTypeFashions[statNum].topBodyColors.Length );
			int bottomBodyColorIndex = Random.Range( 0, _buddyTypeFashions[statNum].bottomBodyColors.Length );

			skinnedMeshRend.material.SetColor( "_TintColor1", _buddyTypeFashions[statNum].topBodyColors[topBodyColorIndex] );
			skinnedMeshRend.material.SetColor( "_TintColor2", _buddyTypeFashions[statNum].bottomBodyColors[bottomBodyColorIndex] );
		}

		skinnedMeshRend.material.SetTexture( "_SkinTex", _skinColors[Random.Range( 0, _skinColors.Length )] );
	}
}
