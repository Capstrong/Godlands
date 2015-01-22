using UnityEngine;
using System.Collections;

public class Beacon : MonoBehaviour 
{
	void Update () 
	{
		transform.LookAt(Vector3.up);
		transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
	}
}
