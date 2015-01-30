using UnityEngine;
using System.Collections;

public class BuddyStats : MonoBehaviour
{
	public Material heartMaterial;

	int currentStats = 0;
	ParticleSystem _particles;

	void Awake()
	{
		_particles = GetComponentInChildren<ParticleSystem>();
	}

	public void GiveResource( ActorPhysics actorPhysics, ResourceData resourceData )
	{
		currentStats++;
		Emote( heartMaterial );
	}

	public void Emote( Material emoteMaterial )
	{
		_particles.Clear();
		_particles.renderer.material = emoteMaterial;
		_particles.Emit( 1 );
	}
}
