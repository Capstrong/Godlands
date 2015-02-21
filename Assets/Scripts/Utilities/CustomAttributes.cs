using UnityEngine;
using System.Collections;

public class ReadOnlyAttribute : PropertyAttribute 
{
	public string displayName = "";

	public ReadOnlyAttribute( string _displayName = "")
	{
		displayName = _displayName;
	}
}
