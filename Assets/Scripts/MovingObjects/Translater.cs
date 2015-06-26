using UnityEngine;
using System.Collections;

public class Translater : MonoBehaviour
{

	[SerializeField] Vector3 _direction = new Vector3();
	[SerializeField] float _speed = 0;

	[Space(10),Header("Duration")]
	[SerializeField] bool _destroyAfterDuration = true;
	[SerializeField] float _duration = 0;

	Vector3 _velocity;

	void Start ()
	{
		if ( _destroyAfterDuration )
		{
			Destroy( gameObject, _duration );
		}
		_velocity = _direction.normalized * _speed;
	}
	
	void Update () 
	{
		transform.Translate( _velocity * Time.deltaTime );
	}
}
