using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour
{
	[SerializeField] string _interactText = "";
	public string interactText
	{
		get { return _interactText; }
	}

	[ToolTip( "In seconds" )]
	[SerializeField] float _duration = 5.0f;
	public float duration
	{
		get { return _duration; }
	}
}
