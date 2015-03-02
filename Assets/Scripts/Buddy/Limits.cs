using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Limits : MonoBehaviour {

	[SerializeField] string _requiredTag;
	[ReadOnly("Capsule Colliders")]
	[SerializeField] List<CapsuleCollider> _colliders;
	public List<CapsuleCollider> colliders
	{
		get { return _colliders; }
	}

	void Start()
	{
		GameObject[] limitObjects = GameObject.FindGameObjectsWithTag( _requiredTag );

		foreach ( GameObject limitObject in limitObjects )
		{
			_colliders.Add( (CapsuleCollider) limitObject.collider );
		}
	}
}
