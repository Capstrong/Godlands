using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class FogOfWar : MonoBehaviour
{
	public GameObject[] cloudPrefabs;
	public GameObject colliderPrefab;

	public float radius;
	public float radiusError;
	public float height;

	[Range( 0.0f, 360.0f )]
	public float cloudIncrement;

	public float minScale;
	public float maxScale;

	public int numClouds;
	public int numColliders;

	private new Transform transform;

	public void Awake()
	{
		transform = GetComponent<Transform>();
	}

	void Start()
	{
		GenerateFogOfWar();
	}

	public void GenerateFogOfWar()
	{
		ClearFogOfWar();
		GenerateClouds();
		GenerateColliders();
	}

	public void ClearFogOfWar( bool inEditor = false )
	{
		// destroy all children
		var children = new List<GameObject>();
		foreach ( Transform child in transform ) children.Add( child.gameObject );
		children.ForEach( child => Destroy( child ) );
	}

#region Editor Helpers
	public void GenerateFogOfWarEditor()
	{
		ClearFogOfWarEditor();
		GenerateClouds();
		GenerateColliders();
	}

	public void ClearFogOfWarEditor()
	{
		// destroy all children
		var children = new List<GameObject>();
		foreach ( Transform child in transform ) children.Add( child.gameObject );
		children.ForEach( child => DestroyImmediate( child ) );
	}
#endregion

	private void GenerateClouds()
	{
		Vector3 baseOffset = Vector3.forward;
		Quaternion rotation = Quaternion.Euler( 0.0f, cloudIncrement, 0.0f );
		// generate a ring of the cloud prefabs
		for ( int count = 0; count < numClouds; ++count )
		{
			// rotate base offset and calculate full offset
			baseOffset = rotation * baseOffset;
			Vector3 cloudOffset = baseOffset * ( radius + Random.Range( 0, radiusError ) )
				+ Vector3.up * Random.Range( 0.0f, height );

			Transform cloud = ( GameObject.Instantiate(
				cloudPrefabs[Random.Range( 0, cloudPrefabs.Length )],
				cloudOffset ,
				Quaternion.identity ) as GameObject ).GetComponent<Transform>();
			cloud.localScale = new Vector3( Random.Range( minScale, maxScale ),
			                                Random.Range( minScale, maxScale ),
			                                Random.Range( minScale, maxScale ) );
			cloud.SetParent( transform, false );
		}
	}

	private void GenerateColliders()
	{
		// build colliders
		float increment = 360.0f / numColliders;
		Vector3 baseOffset = Vector3.forward;
		Quaternion rotation = Quaternion.Euler( 0.0f, increment, 0.0f );
		for ( int count = 0; count < numColliders; ++count )
		{
			baseOffset = rotation * baseOffset;

			Transform collider = ( GameObject.Instantiate(
				colliderPrefab,
				baseOffset * radius + Vector3.up * height * 0.5f,
				Quaternion.LookRotation( baseOffset )
				) as GameObject ).GetComponent<Transform>();
			float width = 2 * radius * Mathf.Sin( Mathf.Deg2Rad * increment * 0.5f );
			collider.localScale = new Vector3( width, height, 1.0f );
			collider.SetParent( transform, false );
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(GetComponent<Transform>().position + Vector3.up * .5f, "S.png", true);
	}
}

[CustomEditor( typeof( FogOfWar ) )]
public class FoWEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		FogOfWar fogOfWar = (FogOfWar)target;
		fogOfWar.Awake();

		if ( GUILayout.Button( "Generator Fog of War" ) )
		{
			fogOfWar.GenerateFogOfWarEditor();
		}

		if ( GUILayout.Button( "Clear Fog of War" ) )
		{
			fogOfWar.ClearFogOfWarEditor();
		}
	}
}
