using UnityEngine;
using System;
using System.Collections;

public class TextMultiVolumeContents : ScriptableObject
{
	[TextArea(3, 10)]
	[SerializeField] string _text = "";
	public string text
	{
		get { return _text; }
	}

	[NonSerialized]
	public bool hasBeenDisplayed = false;
}
