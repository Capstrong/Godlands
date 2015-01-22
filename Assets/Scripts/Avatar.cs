using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Avatar : MonoBehaviour {

//    public Dictionary<ResourceType, int> resourceCount = new Dictionary<ResourceType, int>();
//    public float rotSpeed;
//    public float linearSpeed;
//    public float jumpForce;
//    public GameObject resourcePopPrefab;
//    public Text resourceTextUI;
//    public Text statTextUI;
//
//	// Use this for initialization
//	void Start () {
//        resourceCount[ResourceType.Apple] = 0;
//        resourceCount[ResourceType.Joy] = 0;
//        resourceCount[ResourceType.Creativity] = 0;
//
//        UpdateUI();
//	}
//	
//	// Update is called once per frame
//	void Update () {
//        transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * rotSpeed);
//
//        transform.position += Input.GetAxis("Vertical") * transform.forward * linearSpeed;
//
//        if (Input.GetKeyDown(KeyCode.Space) && transform.position.y < 0.5f)
//        {
//            rigidbody.AddForce(Vector3.up * jumpForce);
//        }
//
//	}
//
//    void OnTriggerEnter(Collider other)
//    {
//        resource resourceComponent = other.gameObject.GetComponent<resource>();
//
//        if (resourceComponent != null)
//        {
//            Debug.Log("resource get");
//            resourceCount[resourceComponent.type] += resourceComponent.amount;
//            Destroy(other.gameObject);
//            Instantiate(resourcePopPrefab, transform.position, Quaternion.identity);
//            UpdateUI();
//        }
//    }
//
//    void UpdateUI()
//    {
//        resourceTextUI.text = "Apples: " + resourceCount[ResourceType.Apple]
//                    + "\nJoy: " + resourceCount[ResourceType.Joy]
//                    + "\nCreativity: " + resourceCount[ResourceType.Creativity];
//
//        statTextUI.text = "Linear Speed: " + linearSpeed
//                        + "\nRotational Speed: " + rotSpeed
//                        + "\nJumpForce: " + jumpForce;
//    }
}
