using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer( typeof( DisplayOnlyAttribute ) )]
public class DisplayOnlyDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
	{
		switch( property.propertyType )
		{
			case SerializedPropertyType.Float: 
				EditorGUI.LabelField( position, property.name, property.floatValue.ToString());
				break;
			case SerializedPropertyType.Integer: 
				EditorGUI.LabelField( position, property.name, property.intValue.ToString());
				break;
			case SerializedPropertyType.Vector3: 
				EditorGUI.LabelField( position, property.name, property.vector3Value.ToString());
				break;
			default: 
				EditorGUI.LabelField( position, label.text, "Unimplemented type. Try implementing it!" );
				break;
		}
	}
}