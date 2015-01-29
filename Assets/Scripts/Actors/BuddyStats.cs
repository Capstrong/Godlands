using UnityEngine;
using System.Collections;

public class BuddyStats : MonoBehaviour
{
	int currentStats = 0;

	public void GiveResource(ActorPhysics actorPhysics, ResourceData resourceData)
	{
		currentStats++;
	}
}
