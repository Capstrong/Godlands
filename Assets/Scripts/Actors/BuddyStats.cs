using UnityEngine;
using System.Collections;

public class BuddyStats : MonoBehaviour
{
	public GodTag owner;

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
		DebugUtils.Assert( _isAlive, "Cannot give a dead buddy resources." );

		currentStats++;
		Emote( heartMaterial );
	}

	public void Emote( Material emoteMaterial )
	{
		_particles.Clear();
		_particles.renderer.material = emoteMaterial;
		_particles.Emit( 1 );
	}

	/**
	 * @brief Kill the buddy.
	 * 
	 * @details
	 *     Killing the buddy means starting the death animation
	 *     and ending (destroying) the behavior tree. Since neither
	 *     of these can be recovered from, killing a buddy is a
	 *     permanant thing and cannot be undone.
	 */
	public void Kill()
	{
		_isAlive = false;
		Destroy( GetComponent<AIController>() );
		GetComponentInChildren<Animator>().SetTrigger( "isDead" );
	}
}
