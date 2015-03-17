using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer( typeof( BuddyResourceCurve ) )]
public class BuddyResourceCurveDrawer : PropertyDrawer
{
	float textHeight = 15f;

	public override float GetPropertyHeight( SerializedProperty property,
	                                         GUIContent label )
	{
		return textHeight * 11; // Count up the number of lines
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		BuddyResourceCurve resourceCurve = (BuddyResourceCurve)fieldInfo.GetValue( property.serializedObject.targetObject );

		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty( position, label, property );

		// Draw label
		position = EditorGUI.PrefixLabel( position, GUIUtility.GetControlID( FocusType.Passive ), label );

		// Don't make child fields be indented
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects
		Rect firstThird = new Rect( position.x, position.y, position.width * .33f, textHeight );
		Rect secondThird = new Rect( position.x + position.width * .33f, position.y, position.width * .33f, textHeight );
		Rect thirdThird = new Rect( position.x + position.width * .66f, position.y, position.width * .33f, textHeight );

		EditorGUI.LabelField( firstThird, "A" );
		EditorGUI.LabelField( secondThird, "B" );
		EditorGUI.LabelField( thirdThird, "C" );

		firstThird.y += textHeight;
		secondThird.y += textHeight;
		thirdThird.y += textHeight;

		// Draw fields - passs GUIContent.none to each so they are drawn without label
		// Drawing them with a label screwed stuff up
		EditorGUI.PropertyField( firstThird, property.FindPropertyRelative( "a" ), GUIContent.none );
		EditorGUI.PropertyField( secondThird, property.FindPropertyRelative( "b" ), GUIContent.none );
		EditorGUI.PropertyField( thirdThird, property.FindPropertyRelative( "c" ), GUIContent.none );

		Rect labelRect = new Rect(position.x, position.y + textHeight * 3, position.width, textHeight);

		EditorGUI.LabelField( labelRect, "Total Resource Cost = " + BuddyResourceCurve.equationString );

		labelRect.y += textHeight;
		labelRect.height *= 2;

		EditorGUI.LabelField( labelRect, "Number of   Total Nightly      Marginal\n" +
		                                 "Buddies       Resource Cost   Cost");

		labelRect.y += textHeight;

		for ( int i = 1; i <= 5; i++ )
		{ 
			labelRect.y += textHeight;

			EditorGUI.LabelField( labelRect, i.ToString() + "                  " + 
			                                 resourceCurve.Evaluate( i ).ToString("0.00") + "                " + 
			                                 ( resourceCurve.Evaluate( i ) - resourceCurve.Evaluate( i - 1 ) ).ToString("0.00"));
		}

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}
