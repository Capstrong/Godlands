using UnityEngine;
using System.Collections;

public class AdultBuddyShaper : MonoBehaviour 
{
	SkinnedMeshRenderer skinnedMeshRend = null;

	void Awake()
	{
		skinnedMeshRend = GetComponentInChildren<SkinnedMeshRenderer>();
	}

	public void SetBuddyStyle( Color clothColorA, Color clothColorB, Color rimColor, float rimPower )
	{
		skinnedMeshRend.material.SetColor( "_TintColor1", clothColorA );
		skinnedMeshRend.material.SetColor( "_TintColor2", clothColorB );
		
		skinnedMeshRend.material.SetFloat( "_RimPower", rimPower );
		skinnedMeshRend.material.SetColor( "_RimColor", rimColor );
	}
}
