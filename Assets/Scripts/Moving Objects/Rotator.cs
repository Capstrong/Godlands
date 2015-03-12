using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour 
{
	[Tooltip("Rate of rotation around each axis in degrees per second.")]
	[SerializeField] Vector3 _rotation = Vector3.zero;

	void Update () 
	{
		transform.rotation *= Quaternion.Euler( _rotation * Time.deltaTime );
	}
}
