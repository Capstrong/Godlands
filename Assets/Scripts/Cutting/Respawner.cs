using UnityEngine;
using System.Collections;

public class Respawner : MonoBehaviour
{
	private SphereCollider testCollider = null;

	private GameObject _objectToRespawn = null;
	public GameObject objectToRespawn
	{
		get 
		{ 
			return _objectToRespawn; 
		}

		set 
		{ 
			_objectToRespawn = value;
			testCollider = _objectToRespawn.GetComponent<SphereCollider>();
		}
	}

	void Update()
	{
		Collider[] colliders = Physics.OverlapSphere( _objectToRespawn.transform.position + testCollider.center, testCollider.radius * objectToRespawn.transform.localScale.x );

		bool collidingWithPlayer = false;

		foreach ( Collider collider in colliders )
		{
			// Get the parent because we are colliding with the lifter or the bumper
			if ( collider.transform.parent
			  && collider.transform.parent.gameObject.GetComponent<GodTag>() )
			{
				collidingWithPlayer = true;
			}
		}

		if ( !collidingWithPlayer )
		{
			objectToRespawn.SetActive( true );
			Destroy( gameObject );
		}
	}
}
