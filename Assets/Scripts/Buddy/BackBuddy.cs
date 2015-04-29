using UnityEngine;
using System.Collections;

public class BackBuddy : MonoBehaviour 
{
	SkinnedMeshRenderer mySkinnedMesh = null;
	MinMaxI copyBlendShapeIndicesRange = new MinMaxI( 3, 12 );

	void Awake()
	{
		mySkinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
	}

	public void CopyBuddy( SkinnedMeshRenderer sourceBuddyMesh )
	{
		for( int i = copyBlendShapeIndicesRange.min; i <= copyBlendShapeIndicesRange.max; i++ )
		{
			mySkinnedMesh.SetBlendShapeWeight( i, sourceBuddyMesh.GetBlendShapeWeight( i ) );
		}

		mySkinnedMesh.material.SetColor( "_TintColor1", sourceBuddyMesh.material.GetColor( "_TintColor1" ) );
		mySkinnedMesh.material.SetColor( "_TintColor2", sourceBuddyMesh.material.GetColor( "_TintColor2" ) );
		
		mySkinnedMesh.material.SetTexture( "_SkinTex", sourceBuddyMesh.material.GetTexture( "_SkinTex" ) );
	}

	Animator animator = null;
	
	public void PlayEvent( string eventName )
	{
		if( !animator )
		{
			animator = GetComponent<Animator>();
		}
		
		animator.Play( eventName, 0 );
	}
}
