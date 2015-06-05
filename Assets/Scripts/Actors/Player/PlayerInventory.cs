using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// types and current count
[System.Serializable] 
public class InventoryDictionary : SerializableDictionary<InventoryItemData, int> { }

[RequireComponent( typeof( PlayerCamera ), typeof( PlayerStats ) )]
public class PlayerInventory : ActorComponent
{
	InventoryScrollBar _inventoryBar;

	[SerializeField] GameObject _resourcePopPrefab = null;

	[SerializeField] List<BuddyTag> _buddies = new List<BuddyTag>();
	public List<BuddyTag> buddies
	{
		get { return _buddies; }
	}

	[SerializeField] PickupBuddyItemData _pickupBuddyItemData = null;
	[SerializeField] float _lookOverrideDuration = 0.5f;

	[ReadOnly("Inventory")]
	[SerializeField] InventoryDictionary _inventory = new InventoryDictionary();

	// currently held types to show on UI bar
	List<InventoryItemData> _heldInventoryItems = new List<InventoryItemData>();

	int _resourceIndex = 0; // Currently selected resource on UI bar
	GameObject _heldResource = null; 

	PlayerActor _playerActor = null;

	[SerializeField] BackBuddy _backBuddy = null;
	public BackBuddy backBuddy
	{
		get { return _backBuddy; }
	}

	bool _isCarryingBuddy = false;
	public bool isCarryingBuddy
	{
		get { return _isCarryingBuddy; }
	}

	[SerializeField] LayerMask _buddyLayer = 0;
	public LayerMask buddyLayer
	{
		get { return _buddyLayer; }
	}

	[SerializeField] float _buddySpawnDistance = 3.0f;

	[SerializeField] float _backBuddyHappinessWaitTime = 0.0f;
	[SerializeField] float _backBuddyHappinessIncrement = 0.0f;

	[SerializeField] float _pickupBuddyTime = 3.5f;
	[SerializeField] float _putDownBuddyTime = 3f;
	[SerializeField] float _spawnBuddyTime = 7f;

	Coroutine _backBuddyHappinessRoutine = null;
	
	AxisButtons _altScrollButton = new AxisButtons("Alt_Scroll");

	public override void Awake()
	{
		base.Awake();

		_playerActor = GetComponent<PlayerActor>();
		_inventory.Add( Instantiate<PickupBuddyItemData>( _pickupBuddyItemData ), 1 );

		_inventoryBar = GameObject.FindObjectOfType<InventoryScrollBar>();
		DebugUtils.Assert( _inventoryBar, "There must be an InventoryScrollBar object in the scene." );

		UpdateResourceList();
	}

	// Use this for initialization
	void Start()
	{
		DayCycleManager.RegisterEndOfDayCallback( ResetBackBuddy );
	}

	// Update is called once per frame
	void Update()
	{
		_altScrollButton.Update();
		CheckScroll();
	}

	void CheckScroll()
	{
		if ( _heldInventoryItems.Count <= 1 )
		{
			// Nothing to scroll
			return;
		}

		float scrollAmount = InputUtils.GetAxis("Scroll");

		if ( WadeUtils.IsZero( scrollAmount ) )
		{
			if ( _altScrollButton.positiveDown )
			{
				scrollAmount = -1f;
			}
			else if ( _altScrollButton.negativeDown )
			{
				scrollAmount = 1f;
			}
		}

		if ( !WadeUtils.IsZero( scrollAmount ) )
		{
			int nextIndex = _resourceIndex + ( scrollAmount < 0f ? -1 : 1 );
			_resourceIndex = MathUtils.Mod( nextIndex, _heldInventoryItems.Count );
		}

		_inventoryBar.UpdateScrollArrows( scrollAmount );
		_inventoryBar.UpdateInventoryBar( _resourceIndex, _heldInventoryItems.ToArray(), _inventory );
	}

	public bool CanUseItemWithoutTarget()
	{
		return !_heldInventoryItems[_resourceIndex].needsTarget;
	}

	public void UseItem()
	{
		_heldInventoryItems[_resourceIndex].UseItem( _playerActor, new RaycastHit() );
	}

	public bool UseItemWithTarget( RaycastHit hitInfo )
	{
		return _heldInventoryItems[_resourceIndex].UseItem( _playerActor, hitInfo );
	}

	public bool CheckGiveResources( RaycastHit hitInfo )
	{
		DebugUtils.Assert( hitInfo.transform != null, "hitInfo must have data." );
		BuddyStats buddyStats = hitInfo.transform.GetComponent<BuddyStats>();

		if ( CheckGiveResources( buddyStats ) )
		{
			// look at the buddy
			_playerActor.physics.OverrideLook(
				buddyStats.GetComponent<Transform>().position - GetComponent<Transform>().position,
				_lookOverrideDuration );
			return true;
		}
		else
		{
			return false;
		}
	}

	bool CheckGiveResources( BuddyStats buddyStats )
	{
		if ( buddyStats && buddyStats.isAlive )
		{
			GiveResource( buddyStats );
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool CheckPickUpBuddy( RaycastHit hitInfo )
	{
		if( hitInfo.transform )
		{
			BuddyStats buddyStats = hitInfo.transform.GetComponent<BuddyStats>();

			if ( buddyStats
			  && buddyStats.isAlive
			  && !_isCarryingBuddy )
			{
				StartCoroutine( PickUpBuddyRoutine( buddyStats ) );
				return true;
			}
		}

		return false;
	}

	IEnumerator PickUpBuddyRoutine( BuddyStats buddyStats )
	{
		_isCarryingBuddy = true;

		_playerActor.physics.OverrideLook( buddyStats.GetComponent<Transform>().position - _playerActor.transform.position, _lookOverrideDuration );
		_playerActor.controls.TimedControlLoss( _pickupBuddyTime );
		_playerActor.animator.Play( "PickupBuddy" );

		yield return new WaitForSeconds( _pickupBuddyTime );

		BuddyShaper buddyShaper = buddyStats.GetComponentInChildren<BuddyShaper>();
		if ( buddyShaper )
		{
			_backBuddy.gameObject.SetActive( true );             // Buddy is always on back, we just hide it
			_backBuddy.CopyBuddy( buddyStats, buddyShaper.skinnedMeshRend ); // Copy buddy style to backBuddy prototype
			_backBuddy.hiddenBuddy.gameObject.SetActive( false );

			if ( _backBuddyHappinessRoutine != null )
			{
				StopCoroutine( _backBuddyHappinessRoutine );
			}

			_backBuddyHappinessRoutine = StartCoroutine( BackBuddyHappinessRoutine( _backBuddy.hiddenBuddy ) );
		}
	}

	IEnumerator BackBuddyHappinessRoutine( BuddyStats buddyStats )
	{
		while ( true )
		{
			// Wait time should be first so that the player cannot spam pick up/put down
			// to get happiness for free
			yield return new WaitForSeconds( _backBuddyHappinessWaitTime );

			buddyStats.AdjustHappiness( _backBuddyHappinessIncrement );
			buddyStats.RecalculateStat();

			if ( buddyStats.happiness >= 1.0f )
			{
				yield break;
			}
		}
	}

	public bool CheckPutDownBuddy()
	{
		if ( !_isCarryingBuddy || 							// If not carrying a buddy
		     !_backBuddy.hiddenBuddy.isAlive || 			// If cloned buddy has died
		     !MathUtils.IsWithinInfiniteVerticalCylinders( transform.position + transform.forward * _buddySpawnDistance, LimitsManager.colliders ) )
		{
			// TODO: Feedback and effect to explain why the buddy can't be spawned outside the garden
			return false;
		}

		StartCoroutine( PutDownBuddyRoutine() );
		return true;
	}

	IEnumerator PutDownBuddyRoutine()
	{
		_isCarryingBuddy = false;

		_playerActor.controls.TimedControlLoss( _putDownBuddyTime );
		_playerActor.animator.Play( "PutDownBuddy" );

		RaycastHit hitInfo;
		Physics.Raycast( new Ray( transform.position, transform.forward), out hitInfo, _buddySpawnDistance );
		Vector3 spawnLocation = ( hitInfo.transform ? hitInfo.point : transform.position + transform.forward * _buddySpawnDistance );

		yield return new WaitForSeconds( _putDownBuddyTime );
		
		_backBuddy.hiddenBuddy.gameObject.SetActive( true );
		_backBuddy.hiddenBuddy.BackReset();
		_backBuddy.hiddenBuddy.gameObject.transform.position = spawnLocation;
		_backBuddy.gameObject.SetActive( false );
		_backBuddy.hiddenBuddy = null;
		
		if ( _backBuddyHappinessRoutine != null )
		{
			StopCoroutine( _backBuddyHappinessRoutine );
		}
	}

	void GiveResource( BuddyStats buddyStats )
	{
		if ( buddyStats.GiveResource( (ResourceData)_heldInventoryItems[_resourceIndex] ) )
		{
			_inventory[_heldInventoryItems[_resourceIndex]]--;
			UpdateResourceList();
		}
	}

	public InventoryItemData GetCurrentItemData()
	{
		return _heldInventoryItems[_resourceIndex];
	}

	public bool SpawnBuddy()
	{
		if ( !MathUtils.IsWithinInfiniteVerticalCylinders( transform.position + transform.forward * _buddySpawnDistance, LimitsManager.colliders ) )
		{
			// TODO: Feedback and effect to explain why the buddy can't be spawned outside the garden
			return false;
		}

		StartCoroutine( SpawnBuddyRoutine() );
		return true;
	}

	IEnumerator SpawnBuddyRoutine()
	{
		_playerActor.controls.TimedControlLoss( _spawnBuddyTime );
		_playerActor.animator.Play( "CreateBuddy" );

		// Spawn buddy and set scale to 0
		RaycastHit hitInfo;
		Physics.Raycast( new Ray( transform.position, transform.forward), out hitInfo, _buddySpawnDistance );
		
		Vector3 spawnLocation = ( hitInfo.transform ? hitInfo.point : transform.position + transform.forward * _buddySpawnDistance );

		BuddyItemData buddyItemData = (BuddyItemData)_heldInventoryItems[_resourceIndex];
		BuddyStats newBuddy = ( Instantiate( buddyItemData.buddyPrefab,
		                                    spawnLocation,
		                                    Quaternion.identity ) as GameObject ).GetComponent<BuddyStats>();
		newBuddy.Initialize( GetComponent<GodTag>(), buddyItemData );
		
		_buddies.Add( newBuddy.GetComponent<BuddyTag>() );
		
		_inventory[_heldInventoryItems[_resourceIndex]]--;
		UpdateResourceList();
		newBuddy.RecalculateStat();

		Transform newBuddyTransform = newBuddy.GetComponent<Transform>();
		newBuddyTransform.localScale = Vector3.zero;

		// Scale buddy up to true size
		float spawnBuddyTimer = 0f;
		while( spawnBuddyTimer < _spawnBuddyTime )
		{
			newBuddyTransform.localScale = Vector3.Lerp( Vector3.zero, Vector3.one, spawnBuddyTimer/_spawnBuddyTime );

			spawnBuddyTimer += Time.deltaTime;
			yield return 0;
		}

		newBuddyTransform.localScale = Vector3.one;
	}

	public void ResetBackBuddy()
	{
		if ( _isCarryingBuddy )
		{
			_backBuddy.hiddenBuddy.gameObject.SetActive( true );
			_backBuddy.Reset();

			_backBuddy.gameObject.SetActive( false );

			if ( _backBuddyHappinessRoutine != null )
			{
				StopCoroutine( _backBuddyHappinessRoutine );
			}

			_isCarryingBuddy = false;
		}
	}

	public void HideBackBuddy()
	{
		if ( _isCarryingBuddy )
		{
			//_backBuddy.hiddenBuddy.gameObject.SetActive( true );
			_backBuddy.Reset();
			_backBuddy.gameObject.SetActive( false );
			
			if ( _backBuddyHappinessRoutine != null )
			{
				StopCoroutine( _backBuddyHappinessRoutine );
			}
			
			_isCarryingBuddy = false;
		}
	}

	public void PickupItem( InventoryItemData itemData )
	{
		if ( !_inventory.ContainsKey( itemData ) )
		{
			_inventory[itemData] = 0;
		}

		_inventory[itemData]++;
		UpdateResourceList();
	}

	void UpdateResourceList()
	{
		_heldInventoryItems.Clear();

		foreach ( InventoryItemData itemData in _inventory.Keys )
		{
			if ( _inventory[itemData] > 0 )
			{
				_heldInventoryItems.Add( itemData );
			}
		}

		_resourceIndex = ( _heldInventoryItems.Count > 0 ? _resourceIndex % _heldInventoryItems.Count : 0 );

		if ( _heldInventoryItems.Count == 0 )
		{
			if ( _heldResource )
			{
				Destroy( _heldResource );
			}

			_inventoryBar.NullInventoryBar();
		}
		else if ( _heldInventoryItems.Count > 0 )
		{
			_inventoryBar.UpdateInventoryBar( _resourceIndex, _heldInventoryItems.ToArray(), _inventory );
		}
	}

	void OnTriggerEnter( Collider other )
	{
		InventoryPickupItem inventoryItem = other.gameObject.GetComponentInChildren<InventoryPickupItem>();
		if ( inventoryItem && !inventoryItem.used )
		{
			inventoryItem.Use();

			PickupItem( inventoryItem.resourceData );

			WadeUtils.TempInstantiate( _resourcePopPrefab, other.transform.position, Quaternion.identity, 1f );
		}
	}
}
