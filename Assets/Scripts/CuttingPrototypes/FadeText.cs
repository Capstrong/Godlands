using UnityEngine;
using System.Collections;

public class FadeText : MonoBehaviour 
{
	[SerializeField] MinMaxF _fadeRange = new MinMaxF( 10f, 15f);
	TextMesh _textMesh;
	Transform _playerCam;

	void Awake()
	{
		_textMesh = GetComponent<TextMesh>();
		_playerCam = GameObject.FindObjectOfType<Camera>().transform;
	}

	void Update()
	{
		float curDist = Vector3.Distance( transform.position, _playerCam.position );

		Color meshColor = _textMesh.color;
		meshColor.a = Mathf.Lerp( 1f, 0f, Mathf.InverseLerp( _fadeRange.min, _fadeRange.max, curDist) );
		_textMesh.color = meshColor;
	}
}
