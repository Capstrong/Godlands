using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Renderer ) )]
public class ScrollUVs : MonoBehaviour 
{
	[SerializeField] Vector2 uvSpeed = Vector2.zero;
	private Vector2 _currentOffset;
	private Renderer _renderer;

	void Awake () 
	{
		_renderer = GetComponent<Renderer>();
		_currentOffset = _renderer.material.GetTextureOffset("_MainTex");
	}

	void Update () 
	{
		_currentOffset += uvSpeed * Time.deltaTime;
		_renderer.material.SetTextureOffset("_MainTex", _currentOffset);
	}
}
