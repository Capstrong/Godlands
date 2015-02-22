using UnityEngine;
using System.Collections;

public class BuddyStats : MonoBehaviour
{
	public Stat statType = Stat.Invalid;
	[SerializeField] int startingApples = 10;
	[SerializeField] int apples = 0;

	PlayerStats _ownerStats = null;

	[SerializeField] Material _heartMaterial = null;
	[SerializeField] Material _sadMaterial = null;

	[SerializeField] GodTag _owner = null;
	public GodTag owner
	{
		get { return _owner; }

		set
		{
			_owner = value;
			if ( _owner )
			{
				_ownerStats = _owner.gameObject.GetComponent<PlayerStats>();
			}
		}
	}

	[Tooltip( "Used for setting the color of the buddy when it is created." )]
	[SerializeField] MeshRenderer _bodyRenderer = null;
	public MeshRenderer bodyRenderer
	{
		get { return _bodyRenderer; }
	}

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
		apples = startingApples;
		_particles = GetComponentInChildren<ParticleSystem>();
		owner = _owner; // For owners set in the inspector
	}

	static string[] names = {"Longnose", "Jojo", "JillyJane", "Sunshine", "Moosejaw",
	                         "Crabknuckle", "Happy Hairy", "TootBaloot", "Rojina"};
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

	public void GiveResource( PlayerStats actorStats, ResourceData resourceData)
	{
		DebugUtils.Assert( _isAlive, "Cannot give a dead buddy resources." );

		apples++;

		actorStats.IncrementMaxStat( statType );
		Emote( _heartMaterial );
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
