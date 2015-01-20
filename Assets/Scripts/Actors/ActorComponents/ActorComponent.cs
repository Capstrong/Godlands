using UnityEngine;
using System.Collections;

public class ActorComponent : MonoBehaviour 
{
	[HideInInspector]
	public Actor actor;

	public void SetActor(Actor actor)
	{
		this.actor = actor;
	}
}
