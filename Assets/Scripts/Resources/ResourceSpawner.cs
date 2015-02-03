using UnityEngine;
using System.Collections;

public class ResourceSpawner : MonoBehaviour 
{
	public GameObject resourcePrefab;
	[SerializeField] GameObject beaconPrefab;
	
	public int total;
	public float radius;
	
	// Use this for initialization
	void Start () {
		
		for (int i = 0; i < total; i++)
		{
			Vector3 position = transform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)) * radius;
			
			GameObject resource = WadeUtils.Instantiate(resourcePrefab, position, Quaternion.identity);
			resource.AddComponent<Rotator>();

			GameObject beacon = WadeUtils.Instantiate(beaconPrefab);
			beacon.transform.parent = resource.transform;
			WadeUtils.ResetTransform(beacon.transform, true);
		}
	}
	
	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position + Vector3.up * .5f, "ResourceSpawner.png", true);
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}
}
