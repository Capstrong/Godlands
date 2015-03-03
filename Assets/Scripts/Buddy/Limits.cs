using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Limits : MonoBehaviour {

	[SerializeField] string _requiredTag;
	[ReadOnly("Capsule Colliders")]
	[SerializeField] CapsuleCollider[] _colliders;
	public CapsuleCollider[] colliders
	{
		get { return _colliders; }
	}

	void Start()
	{
		_colliders = ( from go in GameObject.FindGameObjectsWithTag( _requiredTag )
		               select( go.collider as CapsuleCollider ) ).ToArray<CapsuleCollider>();
	}
}
