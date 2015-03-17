using UnityEngine;
using System.Collections;

public class BuddyStats : ActorComponent
{
	[SerializeField] int _startingResourceCount = 10;
	[ReadOnly("Current Resources")]
	[SerializeField] int _resources = 0;
	[SerializeField] MinMaxI _idealResourcesRange = new MinMaxI();
	[SerializeField] float _nightlyHappinessDecrement = 0f;
	[Tooltip( "Amount of that happiness immediately increased by when given a resource" )]
	[SerializeField] float _happinessIncrementPerResource = 0f;

	[Tooltip( "Below the min is sad, within the bounds is neutral and above the max is happy." )]
	[SerializeField] MinMaxF _neutralHappinessRange = new MinMaxF();

	[SerializeField] float _startingHappiness = 0f;
	[ReadOnly("Happiness")]
	[SerializeField] float _happiness = 0f;
	public float happiness
	{
		get { return _happiness; }
	}

	PlayerStats _ownerStats = null;

	[SerializeField] AudioSource _stomachRumbleSound = null;
	[SerializeField] AudioSource _happySound   = null;
	[SerializeField] AudioSource _neutralSound = null;
	[SerializeField] AudioSource _sadSound     = null;

	[ReadOnly("Current Happiness State")]
	[SerializeField] AudioSource _currentHappinessSound = null;
	[ReadOnly("Current Happiness Sound")]
	[SerializeField] HappinessState _currentHappinessState = HappinessState.Invalid;

	[SerializeField] Material _sadMaterial     = null;
	[SerializeField] Material _happyMaterial   = null;
	[SerializeField] Material _neutralMaterial = null;
	[SerializeField] Material _hungryMaterial  = null;
	[SerializeField] Material _fullMaterial    = null;
	[SerializeField] Material _overFedMaterial = null;

	[Tooltip("Time in seconds between happiness/hunger emotes")]
	[SerializeField] float _emoteRoutineWait = 0f;
	Coroutine _currentEmoteRoutine = null;

	[SerializeField] ParticleSystem _particles = null;
	[SerializeField] ParticleSystem _deadParticleSystem = null;
	Renderer _particlesRenderer = null;

	[ReadOnly( "Buddy Item Data" )]
	public BuddyItemData itemData = null;

	uint ID = 0;

	enum HappinessState
	{
		Invalid,
		Sad,
		Neutral,
		Happy,
	}

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
	[SerializeField] SkinnedMeshRenderer _bodyRenderer = null;
	public SkinnedMeshRenderer bodyRenderer
	{
		get { return _bodyRenderer; }
	}

	[ReadOnly( "Is Alive" )]
	[SerializeField] bool _isAlive = true;
	public bool isAlive
	{
		get { return _isAlive; }
	}

	[Space( 10 ), Header( "Debug Settings" )]
	[SerializeField] bool _disableStatDecrease = false;

	public override void Awake()
	{
		base.Awake();

		ID = GetID(); // Grab the current unique ID
		name = "Buddy " + GetRandomName( ID );
		_resources = _startingResourceCount;
		_particlesRenderer = _particles.GetComponent<Renderer>();
		owner = GameObject.FindObjectOfType<GodTag>();
		_happiness = _startingHappiness;
		RestartEmoteRoutine();
	}

	public void Initialize( GodTag godTag, BuddyItemData buddyItemData )
	{
		itemData = buddyItemData;
		owner = godTag;

		bodyRenderer.material.color = buddyItemData.statColor;

		BuddyManager.RegisterBuddy( this );
		AdjustHappiness( 0 ); // To initialize sound and stats and stuff
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
		DebugUtils.Assert( resourceData, "Trying to give null resource" );

		_resources++;

		if ( _resources < _idealResourcesRange.min )
		{
			// Hungry
			Emote( _hungryMaterial );
			SoundManager.Play3DSoundAtPosition( _stomachRumbleSound, transform.position );
			AdjustHappiness( _happinessIncrementPerResource );
		}
		else if ( _resources > _idealResourcesRange.max )
		{
			// Overfed
			Emote( _overFedMaterial );
		}
		else
		{
			// Full
			AdjustHappiness( _happinessIncrementPerResource );
			Emote( _fullMaterial );
		}

		RestartEmoteRoutine();
	}

	public void NightlyEvent( int resourceDrain )
	{
		DecrementResources( resourceDrain );
		AffectHappinessWithHunger();
	}

	public void DecrementResources( int resourceDrain )
	{
		_resources -= resourceDrain;

		if ( _resources <= 0 )
		{
			Kill();
		}
	}

	public void AffectHappinessWithHunger()
	{
		if ( !_disableStatDecrease )
		{
			if ( _idealResourcesRange.IsOutside( _resources ) )
			{
				AdjustHappiness( -_nightlyHappinessDecrement );
			}
		}
	}

	public void RecalculateStat()
	{
		if ( !_disableStatDecrease )
		{
			BuddyManager.RecalculateStat( itemData.stat, _ownerStats );
		}
	}

	public void AdjustHappiness( float deltaHappiness )
	{
		_happiness += deltaHappiness;

		if ( _happiness < 0 )
		{
			_happiness = 0;
		}

		RecalculateStat();

		if ( _happiness < _neutralHappinessRange.min
			 && _currentHappinessState != HappinessState.Sad )
		{
			// Sad
			_currentHappinessState = HappinessState.Sad;
			if ( _currentHappinessSound )
			{
				_currentHappinessSound.Stop();
			}
			_currentHappinessSound = SoundManager.Play3DSoundAndFollow( _sadSound, transform );
		}
		else if ( _happiness > _neutralHappinessRange.max
				  && _currentHappinessState != HappinessState.Happy )
		{
			// Happy
			_currentHappinessState = HappinessState.Happy;
			if ( _currentHappinessSound )
			{
				_currentHappinessSound.Stop();
			}
			_currentHappinessSound = SoundManager.Play3DSoundAndFollow( _happySound, transform );
		}
		else if ( _currentHappinessState != HappinessState.Neutral )
		{
			// Neutral
			_currentHappinessState = HappinessState.Neutral;
			if ( _currentHappinessSound )
			{
				_currentHappinessSound.Stop();
			}
			_currentHappinessSound = SoundManager.Play3DSoundAndFollow( _neutralSound, transform );
		}
	}

	void RestartEmoteRoutine()
	{
		if ( _currentEmoteRoutine != null )
		{
			StopCoroutine( _currentEmoteRoutine );
		}

		_currentEmoteRoutine = StartCoroutine( EmoteHappinessRoutine() );
	}

	IEnumerator EmoteHappinessRoutine()
	{
		// Whether happiness or hunger was emoted last
		bool didHappinessEmoteLast = MathUtils.RandBool();

		while ( true )
		{
			yield return new WaitForSeconds( _emoteRoutineWait );

			if ( didHappinessEmoteLast )
			{
				if ( _resources < _idealResourcesRange.min )
				{
					Emote( _hungryMaterial );
				}
				else if ( _resources > _idealResourcesRange.max )
				{
					Emote( _overFedMaterial );
				}
				else
				{
					Emote( _fullMaterial );
				}

				didHappinessEmoteLast = false;
			}
			else
			{
				if ( _happiness < _neutralHappinessRange.min )
				{
					Emote( _sadMaterial );
				}
				else if ( _happiness > _neutralHappinessRange.max )
				{
					Emote( _happyMaterial );
				}
				else
				{
					Emote( _neutralMaterial );
				}

				didHappinessEmoteLast = true;
			}
		}
	}

	public void Emote( Material emoteMaterial )
	{
		_particles.Clear();
		_particlesRenderer.material = emoteMaterial;
		_particles.Emit( 1 );
	}

	public void EmoteDeath()
	{
		_deadParticleSystem.Clear();
		_deadParticleSystem.Emit( 1 );
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
		_happiness = 0;
		RecalculateStat();
		EmoteDeath();
		actor.physics.ChangeState( PhysicsStateType.Dead );

		itemData.respawnItem.Enable(); // Respawn the egg in the world to be gathered again

		StopCoroutine( _currentEmoteRoutine );
		GetComponentInChildren<Animator>().SetTrigger( "isDead" );
	}
}
