using UnityEngine;
using System.Collections;

public class BuddyStats : MonoBehaviour
{
	public Stat statType = Stat.Invalid;
	[SerializeField] int _startingResourceCount = 10;
	[ReadOnly("Current Resources")]
	[SerializeField] int _resources = 0;
	[SerializeField] float _statPerResource = 1.0f;

	PlayerStats _ownerStats = null;

	[SerializeField] AudioSource _decrementStatSound = null;
	[SerializeField] Material _heartMaterial = null;
	[SerializeField] Material _sadMaterial = null;

	GodTag _owner = null;
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

	[ReadOnly( "Is Alive" )]
	[SerializeField] bool _isAlive = true;
	public bool isAlive
	{
		get { return _isAlive; }
	}

	[Header( "Debug Settings" )]
	[SerializeField] bool _disableStatDecrease = false;

	ParticleSystem _particles;

	uint ID = 0;

	void Awake()
	{
		ID = GetID(); // Grab the current unique ID
		name = "Buddy " + GetRandomName( ID );
		_resources = _startingResourceCount;
		_particles = GetComponentInChildren<ParticleSystem>();
		owner = GameObject.FindObjectOfType<GodTag>();
		BuddyManager.RegisterBuddy( this );
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

		_resources++;

		actorStats.SetMaxStat( statType, _resources * _statPerResource );
		Emote( _heartMaterial );
	}

	public void DecrementResources()
	{
		_resources--;

		Emote( _sadMaterial );
		SoundManager.Play3DSoundAtPosition( _decrementStatSound, transform.position );

		if ( !_disableStatDecrease )
		{
			_ownerStats.SetMaxStat( statType, _resources * _statPerResource );
		}

		if ( _resources <= 0 )
		{
			Kill();
		}
	}

	public void RecalculateStatValue()
	{
		_ownerStats.SetMaxStat( statType, _resources * _statPerResource );
	}

	public void Emote( Material emoteMaterial )
	{
		_particles.Clear();
		_particles.GetComponent<Renderer>().material = emoteMaterial;
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
