using UnityEngine;
using System.Collections;

public class ScrollUVs : MonoBehaviour 
{
	Vector2 currentOffset;
	[SerializeField] Vector2 uvSpeed = Vector2.zero;

	void Awake () 
	{
		currentOffset = GetComponent<Renderer>().material.GetTextureOffset("_MainTex");
	}

	void Update () 
	{
		currentOffset += uvSpeed * Time.deltaTime;
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex", currentOffset);
	}
}
