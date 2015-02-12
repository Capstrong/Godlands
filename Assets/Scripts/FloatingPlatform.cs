using UnityEngine;
using System.Collections;

public class FloatingPlatform : MonoBehaviour 
{
	[SerializeField] MinMaxF _floatHeightRange = new MinMaxF( 0.5f, 2f );

	float _floatHeight = 0f;
	float _floatOffsetAmount = 0f; // This makes it so things don't float in tandem

	Vector3 _initPos;

	void Awake()
	{
		_floatHeight = Random.Range( _floatHeightRange.min, _floatHeightRange.max );
		_floatOffsetAmount = Random.value;

		_initPos = transform.position;
	}

	void Update()
	{
		transform.SetPositionY( _initPos.y + Mathf.Sin( Time.timeSinceLevelLoad + _floatOffsetAmount ) * _floatHeight );
	}
}
