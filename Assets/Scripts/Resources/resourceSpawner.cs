using UnityEngine;
using System.Collections;

public class resourceSpawner : MonoBehaviour 
{
	public GameObject resourcePrefab;
	[SerializeField] GameObject beaconPrefab;
	
	public int total;
	public float radius;
	
	// Use this for initialization
	void Start () {
		
		for (int i = 0; i < total; i++)
		{
			Vector3 position = transform.position + Random.insideUnitSphere * radius;
			position.y = 0;
			
			GameObject resource = WadeUtils.Instantiate(resourcePrefab, position, Quaternion.identity);
			resource.AddComponent<rotator>();

			GameObject beacon = WadeUtils.Instantiate(beaconPrefab);
			beacon.transform.parent = resource.transform;
			WadeUtils.ResetTransform(beacon.transform, true);
		}
	}
	
	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position + Vector3.up * .5f, "S.png", true);
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}
}
