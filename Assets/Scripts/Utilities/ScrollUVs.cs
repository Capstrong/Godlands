using UnityEngine;
using System.Collections;

public class ScrollUVs : MonoBehaviour 
{
	Vector2 currentOffset;
	[SerializeField] Vector2 uvSpeed;

	void Awake () 
	{
		currentOffset = renderer.material.GetTextureOffset("_MainTex");
	}

	void Update () 
	{
		currentOffset += uvSpeed * Time.deltaTime;
		renderer.material.SetTextureOffset("_MainTex", currentOffset);
	}
}
