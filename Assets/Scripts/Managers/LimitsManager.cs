using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LimitsManager : SingletonBehaviour<LimitsManager> {

	[ReadOnly("Capsule Colliders")]
	[SerializeField] CapsuleCollider[] _colliders;
	public static CapsuleCollider[] colliders
	{
		get { return instance._colliders; }
	}

	void Start()
	{
		_colliders = ( from go in GameObject.FindObjectsOfType<BuddyLimitsTag>()
		               select( (CapsuleCollider) go.GetComponent<Collider>() ) ).ToArray<CapsuleCollider>();
	}
}
