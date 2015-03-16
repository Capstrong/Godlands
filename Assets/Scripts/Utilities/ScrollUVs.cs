using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Renderer ) )]
public class ScrollUVs : MonoBehaviour 
{
	[SerializeField] string _textureName = "_MainTex";
	[SerializeField] Vector2 _uvSpeed = Vector2.zero;
	private Vector2 _currentOffset;
	private Renderer _renderer;

	void Awake () 
	{
		_renderer = GetComponent<Renderer>();
		_currentOffset = _renderer.material.GetTextureOffset(_textureName);
	}

	void Update () 
	{
		_currentOffset += _uvSpeed * Time.deltaTime;
		_renderer.material.SetTextureOffset(_textureName, _currentOffset);
	}
}
