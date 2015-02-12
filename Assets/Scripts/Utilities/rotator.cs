using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour 
{
	[SerializeField] Vector3 eulerRotation = Vector3.zero;

	void Update () 
	{
		transform.rotation *= Quaternion.Euler( eulerRotation * Time.deltaTime );
	}
}
