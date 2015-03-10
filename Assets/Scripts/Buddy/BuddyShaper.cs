using UnityEngine;
using System.Collections;

public class BuddyShaper : MonoBehaviour 
{
	[SerializeField] int[] overlapBlendIndices = null;
	[SerializeField] int[] exclusiveBlendIndices = null;

	[SerializeField] int legIndex = 0; // Which index of the skinned mesh renderer's blendshapes is associated with leg length
	float legHeightOffsetMod = 0.0006f;

	[SerializeField] MinMaxF exclusiveBlendRange = new MinMaxF( 50f, 100f );

	SkinnedMeshRenderer skinnedMeshRend = null;

	void Awake()
	{
		skinnedMeshRend = GetComponentInChildren<SkinnedMeshRenderer>();
		Debug.Log( skinnedMeshRend.gameObject.name );

		foreach( int i in overlapBlendIndices )
		{
			float setWeight = Random.Range( 0f, 100f );
			skinnedMeshRend.SetBlendShapeWeight( i, setWeight );

			if( i == legIndex )
			{
				transform.position += Vector3.up * setWeight * legHeightOffsetMod;
			}
		}

		int exclusiveBlendIndex = Random.Range( 0, exclusiveBlendIndices.Length );
		skinnedMeshRend.SetBlendShapeWeight( exclusiveBlendIndices[exclusiveBlendIndex], 
		                                     Random.Range( exclusiveBlendRange.min, exclusiveBlendRange.max ) );
	}
}
