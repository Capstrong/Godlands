using UnityEngine;
using System.Collections;
using BehaviorTree;

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

	[SerializeField] int _age = 0;
	[SerializeField] int _adultAge = 5;
	public bool isOfAge
	{
		get
		{
			return _age >= _adultAge;
		}
	}

	// A flag used to catch the first edge of when a buddy flips to being ready to become an adult
	bool isReadyToAgeUp = false;

	[SerializeField] float _startingHappiness = 0f;
	[ReadOnly,Tooltip( "Ranges from 0 to 1" )]
	[SerializeField] float _happiness = 0f;
	public float happiness
	{
		get { return _happiness; }
	}

	[SerializeField] AudioSource _stomachRumbleSound = null;
	[SerializeField] AudioSource _happySound   = null;
	[SerializeField] AudioSource _neutralSound = null;
	[SerializeField] AudioSource _sadSound     = null;

	[ReadOnly("Current Happiness State")]
	[SerializeField] AudioSource _currentHappinessSound = null;
	[ReadOnly("Current Happiness Sound")]
	[SerializeField] HappinessState _currentHappinessState = HappinessState.Invalid;

	[SerializeField] Material _hungerEmoteMaterial = null;

	[SerializeField] Texture[] _hungerEmoteTextures = null;

	[SerializeField] Color _badHungerColor = Color.white;
	[SerializeField] Color _goodHungerColor = Color.white;

	[Tooltip("Time in seconds between happiness/hunger emotes")]
	[SerializeField] float _emoteRoutineWait = 0f;
	Coroutine _currentEmoteRoutine = null;

	[SerializeField] ParticleSystem _emoteParticles = null;
	[SerializeField] ParticleSystem _deadParticles = null;
	[SerializeField] ParticleSystem _adultParticles = null;
	Renderer _particlesRenderer = null;

	[SerializeField] AudioSource _readyToGrowUpSong = null;

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

	enum BuddyState
	{
		Normal,
		Dead,
		Adult,
	}

	GodTag _owner = null;
	public GodTag owner
	{
		get { return _owner; }
	}

	[Tooltip( "Used for setting the color of the buddy when it is created." )]
	[SerializeField] SkinnedMeshRenderer _bodyRenderer = null;
	public SkinnedMeshRenderer bodyRenderer
	{
		get { return _bodyRenderer; }
	}

	[SerializeField] TextMultiVolume _adultText = null;

	[ReadOnly]
	[SerializeField] BuddyState _state = BuddyState.Normal;

	public bool isAlive
	{
		get { return _state == BuddyState.Normal; }
	}

	public bool isAdult
	{
		get { return _state == BuddyState.Adult; }
	}

	[Space( 10 ), Header( "Debug Settings" )]
	[SerializeField] bool _disableStatDecrease = false;


	// Highlighting

	float _defaultRimPower = 4f;
	Color _defaultRimColor = new Color( 0.15f, 0.15f, 0.15f, 1f );

	float _highlightRimPower = 2f;
	Color _highlightRimColor = Color.green;

	bool _isHighlighted = false;

	public override void Awake()
	{
		base.Awake();

		ID = GetID(); // Grab the current unique ID
		name = "Buddy " + GetRandomName( ID );
		_resources = _startingResourceCount;
		_particlesRenderer = _emoteParticles.GetComponent<Renderer>();
		_owner = GameObject.FindObjectOfType<GodTag>();
		_happiness = _startingHappiness;
		RestartEmoteRoutine();

		_hungerEmoteMaterial = Instantiate<Material>( _hungerEmoteMaterial );
	}

	public void Initialize( GodTag godTag, BuddyItemData buddyItemData )
	{
		itemData = buddyItemData;
		_owner = godTag;

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

	public bool IsHungry()
	{
		return _resources < _idealResourcesRange.max;
	}

	public bool GiveResource( ResourceData resourceData )
	{
		DebugUtils.Assert( isAlive, "Cannot give a dead buddy resources." );
		DebugUtils.Assert( resourceData, "Trying to give null resource" );

		if ( _resources < _idealResourcesRange.max )
		{
			_resources++;

			if ( _resources < _idealResourcesRange.min )
			{
				SoundManager.Play3DSoundAtPosition( _stomachRumbleSound, transform.position );
			}

			AdjustHappiness( _happinessIncrementPerResource );
			RecalculateStat();

			UpdateHungerEmoteTexture();
			EmoteHunger( _hungerEmoteMaterial );

			RestartEmoteRoutine();

			return true;
		}
		else
		{
			return false;
		}
	}

	public void AgeUp()
	{
		++_age;
		if ( isOfAge && !isReadyToAgeUp && isAlive )
		{
			isReadyToAgeUp = true;
			_adultParticles.enableEmission = true;
			_adultParticles.Play();
			_adultText.gameObject.SetActive( true );
			SoundManager.Play2DSound( _readyToGrowUpSong );
		}
	}

	public void DecrementResources( int resourceDrain )
	{
		_resources -= resourceDrain;

		if ( _resources <= 0 && isAlive )
		{
			Kill();
		}
	}

	public void AffectHappinessWithHunger()
	{
		if ( !_disableStatDecrease )
		{
			if ( _resources < _idealResourcesRange.min )
			{
				AdjustHappiness( -_nightlyHappinessDecrement );
			}
		}
	}

	public void RecalculateStat()
	{
		if ( !_disableStatDecrease )
		{
			BuddyManager.RecalculateStat( itemData.stat );
		}
	}

	public void AdjustHappiness( float deltaHappiness )
	{
		_happiness += deltaHappiness;

		_happiness = Mathf.Clamp( _happiness, 0f, 1f );

		// Hidden buddy is disabled
		if ( gameObject.activeSelf )
		{
			actor.animator.SetFloat( "happiness", _happiness );
		}

		if ( _happiness < _neutralHappinessRange.min
			 && _currentHappinessState != HappinessState.Sad )
		{
			// Sad
			_currentHappinessState = HappinessState.Sad;
			PlayHappinessSound( _sadSound );
		}
		else if ( _happiness > _neutralHappinessRange.max
				  && _currentHappinessState != HappinessState.Happy )
		{
			// Happy
			_currentHappinessState = HappinessState.Happy;
			PlayHappinessSound( _happySound );
		}
		else if ( _currentHappinessState != HappinessState.Neutral )
		{
			// Neutral
			_currentHappinessState = HappinessState.Neutral;
			PlayHappinessSound( _neutralSound );
		}
	}

	void PlayHappinessSound( AudioSource happinessSound )
	{
		// Sometimes we do this on the hidden buddy which is disabled and can't and shouldn't play sound
		if ( gameObject.activeSelf )
		{
			if ( _currentHappinessSound )
			{
				_currentHappinessSound.Stop();
			}
			_currentHappinessSound = SoundManager.Play3DSoundAndFollow( happinessSound, transform );
		}
	}

	void RestartEmoteRoutine()
	{
		if ( _currentEmoteRoutine != null )
		{
			StopCoroutine( _currentEmoteRoutine );
		}

		_currentEmoteRoutine = StartCoroutine( EmoteHungerRoutine() );
	}

	IEnumerator EmoteHungerRoutine()
	{
		while ( true )
		{
			yield return new WaitForSeconds( _emoteRoutineWait );

			UpdateHungerEmoteTexture();

			EmoteHunger( _hungerEmoteMaterial );
		}
	}

	private void UpdateHungerEmoteTexture()
	{
		// A value from 0 - 1 that is based off how good the buddy feels about its hunger level
		float satisfaction = (float) _resources / _idealResourcesRange.max;

		satisfaction = Mathf.Clamp( satisfaction, 0f, 1f );

		int textureIndex =  Mathf.RoundToInt( satisfaction * ( _hungerEmoteTextures.Length - 1 ) );

		_hungerEmoteMaterial.mainTexture = _hungerEmoteTextures[ textureIndex ];

		_hungerEmoteMaterial.color = Color.Lerp( _badHungerColor, _goodHungerColor, satisfaction );
	}

	public void EmoteHunger( Material emoteMaterial )
	{
		_emoteParticles.Clear();
		_particlesRenderer.material = emoteMaterial;
		_emoteParticles.Emit( 1 );
	}

	public void EmoteDeath()
	{
		_deadParticles.Clear();
		_deadParticles.Emit( 1 );
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
		_state = BuddyState.Dead;
		Destroy( GetComponent<AIController>() );
		_happiness = 0;
		RecalculateStat();
		EmoteDeath();
		actor.physics.ChangeState( PhysicsStateType.Dead );

		RespawnEgg();

		StopCoroutine( _currentEmoteRoutine );

		if ( actor.animator.isActiveAndEnabled )
		{
			actor.animator.SetTrigger( "isDead" );
		}

		if ( isOfAge )
		{
			_adultParticles.Stop();
			_adultParticles.enableEmission = false;
		}
	}

	void RespawnEgg()
	{
		// Debug spawned buddies don't have eggs to respawn
		if ( itemData && itemData.respawnItem )
		{
			// Respawn the egg in the world to be gathered again
			itemData.respawnItem.Enable();
		}
	}

	public void BecomeAdult()
	{
		_state = BuddyState.Adult;
		gameObject.SetActive( false );
		Destroy( GetComponent<AIController>() );
		RespawnEgg();
	}

	// This takes care of everything that needs to be updated after the buddy is
	// put down from being on the player character's back
	public void BackReset()
	{
		// This will update the happiness variable on the animator and
		// restart the happiness sound
		if ( isAlive )
		{
			// Don't do this for dead buddies or else they will start up an animation
			AdjustHappiness( 0f );
		}
		else
		{
			// Have to do this here because the animator is not enabled for the hidden buddy when Kill() is called
			if ( actor.animator.isActiveAndEnabled )
			{
				actor.animator.SetTrigger( "isDead" );
			}
		}
	}

	public float hunger
	{
		get
		{
			return (float)_resources / (float)_idealResourcesRange.max;
		}
	}

	public void SetHighlight()
	{
		if( !_isHighlighted && isAlive )
		{
			bodyRenderer.material.SetFloat( "_RimPower", _highlightRimPower );
			bodyRenderer.material.SetColor( "_RimColor", _highlightRimColor );

			_isHighlighted = true;
		}
	}

	public void RemoveHighlight()
	{
		if( _isHighlighted )
		{
			bodyRenderer.material.SetFloat( "_RimPower", _defaultRimPower );
			bodyRenderer.material.SetColor( "_RimColor", _defaultRimColor );

			_isHighlighted = false;
		}
	}
}
