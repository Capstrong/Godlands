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
	[SerializeField] BuddyStats _buddyStats = null;

	// Blend Shapes
	[SerializeField] int[] _overlapBlendIndices = null;
	[SerializeField] int[] _exclusiveBlendIndices = null;

	[SerializeField] int _legIndex = 0; // Which index of the skinned mesh renderer's blendshapes is associated with leg length
	float _legHeightOffsetMod = 0.0006f;

	[SerializeField] MinMaxF _exclusiveBlendRange = new MinMaxF( 50f, 100f );
	SkinnedMeshRenderer _skinnedMeshRend = null;

	// Scaling
	[SerializeField] MinMaxF _heightScaleRange = new MinMaxF( 0.8f, 1.2f );

	// Coloring
	[SerializeField] BuddyTypeFashion[] _buddyTypeFashions = null;
	[SerializeField] Texture2D[] _skinColors = null;

	[SerializeField] int _numBlendShapes = 0;

	Vector3 _initPos = Vector3.zero;
	Vector3 _initScale = Vector3.one;

	[SerializeField] bool _debugRandomizer = false;

	void Start()
	{
		_skinnedMeshRend = GetComponentInChildren<SkinnedMeshRenderer>();

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

	void RandomizeBuddy()
	{
		for( int i = 0; i < _numBlendShapes; i++ )
		{
			_skinnedMeshRend.SetBlendShapeWeight( i, 0f );
		}

		if( _numBlendShapes > 0 )
		{
			AdjustBlendShapes();
		}

		AdjustSize();
		AdjustColor();
	}

	void AdjustBlendShapes()
	{
		foreach( int i in _overlapBlendIndices )
		{
			float setWeight = Random.Range( 0f, 100f );
			_skinnedMeshRend.SetBlendShapeWeight( i, setWeight );

			if( i == _legIndex )
			{
				transform.position = _initPos + Vector3.up * setWeight * _legHeightOffsetMod;
			}
		}

		int exclusiveBlendIndex = Random.Range( 0, _exclusiveBlendIndices.Length );
		_skinnedMeshRend.SetBlendShapeWeight( 	_exclusiveBlendIndices[exclusiveBlendIndex],
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
		if( _buddyStats )
		{
			int statNum = (int)_buddyStats.itemData.stat;
			int topBodyColorIndex = Random.Range( 0, _buddyTypeFashions[statNum].topBodyColors.Length );
			int bottomBodyColorIndex = Random.Range( 0, _buddyTypeFashions[statNum].bottomBodyColors.Length );

			_skinnedMeshRend.material.SetColor( "_TintColor1", _buddyTypeFashions[statNum].topBodyColors[topBodyColorIndex] );
			_skinnedMeshRend.material.SetColor( "_TintColor2", _buddyTypeFashions[statNum].bottomBodyColors[bottomBodyColorIndex] );

			_skinnedMeshRend.material.SetTexture( "_SkinTex", _skinColors[Random.Range( 0, _skinColors.Length )] );
		}
	}
}
