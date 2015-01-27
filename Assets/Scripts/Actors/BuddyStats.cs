using UnityEngine;
using System.Collections;

public class BuddyStats : MonoBehaviour 
{
	int currentStats = 0;
	[SerializeField] float jumpForce;

	void Awake()
	{
	}

	public void GiveResource(ActorPhysics actorPhysics, ResourceData resourceData)
	{
		currentStats++;
		rigidbody.AddForce(transform.up * jumpForce);
		actorPhysics.jumpForce *= 1.15f;
	}
}
