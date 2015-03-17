using UnityEngine;
using System.Collections;

public class CheckpointLifter : MonoBehaviour 
{
	[SerializeField] float moveSpeed = 5f;

	Vector3 initPos = Vector3.zero;
	[SerializeField] float maxHeightOffset = 25f;

	bool isActive = false;
	bool isRising = false;

	Renderer rend;

	void Awake()
	{
		initPos = transform.position;

		rend = GetComponent<Renderer>();
		rend.material.SetColor( "_EmissionColor", Color.black);
	}

	void FixedUpdate()
	{
		if( isActive )
		{
			if( isRising )
			{
				if( transform.position.y < initPos.y + maxHeightOffset )
				{
					transform.position += Vector3.up * moveSpeed * Time.deltaTime;
				}
				else
				{
					transform.position = initPos + Vector3.up * maxHeightOffset;
				}
			}
			else if( transform.position.y > initPos.y )
			{
				transform.position -= Vector3.up * moveSpeed * Time.deltaTime;
			}
			else
			{
				transform.position = initPos;
			}
		}
	}

	public void Activate()
	{
		isActive = true;
		rend.material.SetColor( "_EmissionColor", Color.white );
	}

	void OnTriggerStay( Collider otherCol )
	{
		if ( otherCol.GetComponentInParent<PlayerActor>() )
		{
			isRising = true;
		}
	}

	void OnTriggerExit( Collider otherCol )
	{
		if ( otherCol.GetComponentInParent<PlayerActor>() )
		{
			isRising = false;
		}
	}
}
