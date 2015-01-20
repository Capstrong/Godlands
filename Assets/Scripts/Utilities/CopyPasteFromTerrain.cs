using UnityEngine;
using System.Collections;

public class CopyPasteFromTerrain : MonoBehaviour 
{
	[SerializeField] Terrain terrain;

	void Start ()
	{
		GameObject[] prefabInstances = new GameObject[terrain.terrainData.treePrototypes.Length];

		for(int i = 0; i < terrain.terrainData.treePrototypes.Length; i++)
		{
			prefabInstances[i] = terrain.terrainData.treePrototypes[i].prefab;
		}

		foreach(TreeInstance tree in terrain.terrainData.treeInstances)
		{
			Instantiate(prefabInstances[tree.prototypeIndex], 
			            Vector3.Scale(tree.position, terrain.terrainData.size) + terrain.GetPosition(),
			            Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f));
		}

		//terrain.enabled = false;
	}
}
