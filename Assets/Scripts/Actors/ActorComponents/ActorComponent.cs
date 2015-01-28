using UnityEngine;
using System.Collections;

public class ActorComponent : MonoBehaviour 
{
	[HideInInspector]
	public Actor actor;

	public virtual void Awake()
	{
		actor = GetComponent<Actor>();
	}
}
