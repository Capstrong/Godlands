using UnityEngine;
using System.Collections;

public class BuddyShaper : MonoBehaviour 
{
	// Blend Shapes
	[SerializeField] int[] overlapBlendIndices = null;
	[SerializeField] int[] exclusiveBlendIndices = null;

	[SerializeField] int legIndex = 0; // Which index of the skinned mesh renderer's blendshapes is associated with leg length
	float legHeightOffsetMod = 0.0006f;

	[SerializeField] MinMaxF exclusiveBlendRange = new MinMaxF( 50f, 100f );
	SkinnedMeshRenderer skinnedMeshRend = null;

	// Scaling
	[SerializeField] MinMaxF heightScaleRange = new MinMaxF( 0.8f, 1.2f );

	// Coloring
	[SerializeField] MinMaxF colorOffsetRange = new MinMaxF( 0f, 0.15f );
	[SerializeField] Color[] skinColors = null;

	void Awake()
	{
		skinnedMeshRend = GetComponentInChildren<SkinnedMeshRenderer>();

		AdjustBlendShapes();
		AdjustSize();
		AdjustColor();
	}

	void AdjustBlendShapes()
	{
		foreach ( int i in overlapBlendIndices )
		{
			float setWeight = Random.Range( 0f, 100f );
			skinnedMeshRend.SetBlendShapeWeight( i, setWeight );

			if ( i == legIndex )
			{
				transform.position += Vector3.up * setWeight * legHeightOffsetMod;
			}
		}

		if ( exclusiveBlendIndices.Length > 0 )
		{
			int exclusiveBlendIndex = Random.Range( 0, exclusiveBlendIndices.Length );
			skinnedMeshRend.SetBlendShapeWeight( exclusiveBlendIndices[exclusiveBlendIndex],
			                                     Random.Range( exclusiveBlendRange.min, exclusiveBlendRange.max ) );
		}
	}

	void AdjustSize()
	{
		transform.localScale =
		    transform.localScale.SetY( heightScaleRange.random );
	}

	void AdjustColor()
	{
		Color colorOffset = new Color( -Mathf.Clamp01( colorOffsetRange.random ),
		                               Mathf.Clamp01( colorOffsetRange.random ),
		                               Mathf.Clamp01( colorOffsetRange.random ),
		                               0f );

		skinnedMeshRend.material.color += colorOffset;
		skinnedMeshRend.material.SetColor( "_SkinColor", skinColors[Random.Range( 0, skinColors.Length )] );
	}
}
