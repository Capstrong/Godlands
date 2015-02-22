using UnityEngine;
using System.Collections;

public class BuddyStats : MonoBehaviour
{
	[SerializeField] Stat statType = Stat.Invalid;
	[SerializeField] bool canDecreaseStat = true;
	[SerializeField] int startingResources = 10;
	[ReadOnly("Current Resources")]
	[SerializeField] int resources = 0;

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
		resources = startingResources;
		_particles = GetComponentInChildren<ParticleSystem>();
		SetGod(owner); // For owners set in the inspector
		BuddyManager.RegisterBuddy( this );
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
		if (Input.GetKeyDown(KeyCode.K))
		{
			Kill();
		}
	}

	public void GiveResource( PlayerActorStats actorStats, ResourceData resourceData)
	{
		DebugUtils.Assert( _isAlive, "Cannot give a dead buddy resources." );

		resources++;

		actorStats.IncrementMaxStat( statType );
		Emote( heartMaterial );
	}

	public void DecrementResources()
	{
		resources--;

		Emote( _sadMaterial );

		if ( canDecreaseStat )
		{
			_actorStats.DecrementMaxStat( statType );
		}

		if ( resources <= 0 )
		{
			Kill();
		}
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
