using UnityEngine;
using System.Collections;

public class TextVolume : MonoBehaviour
{
	[SerializeField] string _text = "";
	public string text
	{
		get { return _text; }
	}
}
