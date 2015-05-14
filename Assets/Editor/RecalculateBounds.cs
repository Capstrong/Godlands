using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class RecalculateBounds : MonoBehaviour 
{
	[MenuItem( "Utilities/LODGroups/Recalculate Bounds of Selection" )]
	public static void RecalculateBoundsForSelection()
	{	
		List<LODGroup> selectedLODs = new List<LODGroup>();
		foreach( Object obj in Selection.objects )
		{
			if( obj is GameObject )
			{
				LODGroup lodGroup = ((GameObject)obj).GetComponent<LODGroup>();
				selectedLODs.Add( lodGroup );
			}
		}

		foreach( LODGroup lod in selectedLODs )
		{
			lod.RecalculateBounds();
		}
	}
}
