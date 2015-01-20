using UnityEngine;
using System.Collections;

public class resourceSpawner : MonoBehaviour {

    public GameObject resourcePrefab;

    public int total;
    public float radius;

	// Use this for initialization
	void Start () {

        for (int i = 0; i < total; i++)
        {
            Vector3 position = transform.position + Random.insideUnitSphere * radius;
            position.y = 0;
            Instantiate(resourcePrefab, position, Quaternion.identity);
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
