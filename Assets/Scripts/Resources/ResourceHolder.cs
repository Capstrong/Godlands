using UnityEngine;
using System.Collections;

public class ResourceHolder : MonoBehaviour
{
	public GameObject resource;

	[SerializeField] float resourceHeightOffset = 0.17f;
	[SerializeField] float _maxHeight = 12f;
	[SerializeField] LayerMask _collisionLayer = -1;

	[SerializeField] bool _showBeacon = false;
	[SerializeField] Transform _beacon = null;

	Transform _transform;

	Renderer _beaconRenderer = null;
	ParticleSystem _particleSystem = null;

	void Awake()
	{
		_transform = GetComponent<Transform>();
		_beaconRenderer = _beacon.GetComponent<Renderer>();
		_particleSystem = GetComponentInChildren<ParticleSystem>();

		RaycastHit hitInfo;

		Physics.Raycast( new Ray( _transform.position, Vector3.up ), out hitInfo, _maxHeight, _collisionLayer );

		float actualHeight = _maxHeight;

		if ( hitInfo.transform )
		{
			actualHeight = hitInfo.transform.position.y - _transform.position.y;
		}

		_beacon.localScale = _beacon.localScale.SetY( actualHeight );
		_particleSystem.startLifetime = actualHeight / _particleSystem.startSpeed * 0.5f; // TODO find out why particles seem to be moving at 2x speed

		if( !_showBeacon )
		{
			_beaconRenderer.enabled = false;
			_particleSystem.enableEmission = false;
		}

		resource = WadeUtils.Instantiate( resource, Vector3.up * resourceHeightOffset, Quaternion.identity );
		resource.GetComponent<Transform>().SetParent( _transform, false );

		resource.GetComponent<InventoryItem>().Initialize( this );
	}

	public void Disable()
	{
		_beaconRenderer.enabled = false;
		_particleSystem.Stop();
	}

	public void Enable()
	{
		if( _showBeacon )
		{
			_beaconRenderer.enabled = true;
			_particleSystem.Play();
		}
	}
}
