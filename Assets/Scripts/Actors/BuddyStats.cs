using UnityEngine;
using System.Collections;

public class BuddyStats : MonoBehaviour
{
	[SerializeField] Stat statType = Stat.Invalid;
	[SerializeField] bool canDecreaseStamina = true;
	[SerializeField] float decreaseResourcesTime = 60.0f; //seconds
	[SerializeField] float currResourceTimer = 0.0f;
	[SerializeField] int startingApples = 10;
	[SerializeField] int apples = 0;

	public GodTag owner = null;

	PlayerActorStats _actorStats = null;

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
		apples = startingApples;
		_particles = GetComponentInChildren<ParticleSystem>();
		SetGod(owner); // For owners set in the inspector
	}

	public void SetGod( GodTag _godTag )
	{
		if(_godTag)
		{
			owner = _godTag;
			_actorStats = _godTag.gameObject.GetComponent<PlayerActorStats>();
		}
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
		// Replace this with an invoke
		currResourceTimer -= Time.deltaTime;

		if ( currResourceTimer < 0.0f && _isAlive )
		{
			// decrease resources
			apples--;

			if (canDecreaseStamina)
			{
				_actorStats.DecrementMaxStat( statType );
			}
			
			currResourceTimer = decreaseResourcesTime;

			if ( apples <= 0 )
			{
				Kill();
			}
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

		actorStats.IncrementMaxStat( statType );
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
