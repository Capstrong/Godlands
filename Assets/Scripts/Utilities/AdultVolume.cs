using UnityEngine;
using System.Collections;

[RequireComponent( typeof( BoxCollider ) )]
public class AdultVolume : MonoBehaviour
{
	void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Transform transform = GetComponent<Transform>();
		Gizmos.matrix = transform.localToWorldMatrix;

		BoxCollider collider = GetComponent<BoxCollider>();
		Gizmos.DrawWireCube( collider.center, collider.size );
	}

	void OnTriggerStay( Collider other )
	{
		PlayerActor player = other.GetComponentInParent<PlayerActor>();

		if ( player 
			&& player.controls.holdButton )
		{
			BuddyStats buddy = player.inventory.backBuddy.hiddenBuddy;

			if ( buddy && buddy.isOfAge )
			{
				// TODO: Do any vfx for indicating the buddy is an adult.

				AdultManager.SpawnAdult( buddy );
				player.inventory.ResetBackBuddy();
			}
		}
	}
}
