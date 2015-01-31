using UnityEngine;
using System.Collections;

public class BuddyStats : MonoBehaviour
{
	public Material heartMaterial;

	bool _isAlive = true;
	public bool isAlive
	{
		get { return _isAlive; }
	}

	int currentStats = 0;
	ParticleSystem _particles;

	void Awake()
	{
		_particles = GetComponentInChildren<ParticleSystem>();
	}

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.K ) )
		{
			Kill();
		}
	}

	public void GiveResource( ActorPhysics actorPhysics, ResourceData resourceData )
	{
		if ( _isAlive )
		{
			currentStats++;
			Emote( heartMaterial );
		}
	}

	public void Emote( Material emoteMaterial )
	{
		_particles.Clear();
		_particles.renderer.material = emoteMaterial;
		_particles.Emit( 1 );
	}

	public void Kill()
	{
		_isAlive = false;
		Destroy( GetComponent<AIController>() );
		GetComponentInChildren<Animator>().SetTrigger( "isDead" );
	}
}
