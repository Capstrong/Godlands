using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor( typeof( Planter ) )]
public class PlanterEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Planter planter = (Planter)target;
		
		if ( GUILayout.Button( "Replant" ) )
		{
			planter.Plant();
		}
	}
}
