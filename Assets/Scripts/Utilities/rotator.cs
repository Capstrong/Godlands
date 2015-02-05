using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour 
{
	[SerializeField] Vector3 eulerRotation;

	void Update () 
	{
		transform.rotation *= Quaternion.Euler( eulerRotation * Time.deltaTime );
	}
}
