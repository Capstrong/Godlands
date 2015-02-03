using UnityEngine;
using System.Collections;

public class BuddyStats : MonoBehaviour
{
	enum StatNames
	{
		Invalid,
		Stamina,
	}

	[SerializeField] float decreaseResourcesTime = 60.0f; //seconds
	[SerializeField] float currResourceTimer = 0.0f;

	[SerializeField] int apples = 0;

	public GodTag owner;

	public Material heartMaterial;

	bool _isAlive = true;
	public bool isAlive
	{
		get { return _isAlive; }
	}

	ParticleSystem _particles;

	uint ID = 0;

	void Awake()
	{
		ID = GetID(); // Grab the current unique ID
		name = "Buddy " + GetRandomName( ID );
		currResourceTimer = decreaseResourcesTime;
		_particles = GetComponentInChildren<ParticleSystem>();
	}

	static string[] names = { "Longnose", "Jojo", "JillyJane", "Sunshine", "Moosejaw"};
	static uint uniqueID = 1;

	static uint GetID()
	{
		return uniqueID++;
	}

	static string GetRandomName( uint ID )
	{
		int randIndex = Random.Range( 0, names.Length );

		return names[randIndex] + ID;
	}

	void Update()
	{
		currResourceTimer -= Time.deltaTime;

		if (currResourceTimer < 0.0f)
		{
			// decrease resources
			apples--;
			currResourceTimer = decreaseResourcesTime;
		}

		if (Input.GetKeyDown(KeyCode.K))
		{
			Kill();
		}
	}

	public void GiveResource( PlayerActorStats actorStats, ResourceData resourceData)
	{
		DebugUtils.Assert( _isAlive, "Cannot give a dead buddy resources." );

		apples++;

		actorStats.IncrementMaxStamina();
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
