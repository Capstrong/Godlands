using UnityEngine;
using System.Collections;

public class ResourceHolder : MonoBehaviour
{
	public GameObject resource;

	[SerializeField] float resourceHeightOffset = 0.17f;
	[SerializeField] float _maxHeight = 12f;
	[SerializeField] LayerMask _collisionLayer;

	[SerializeField] Transform _beacon;

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

		float normalizedHeight = _maxHeight;

		if ( hitInfo.transform )
		{
			normalizedHeight = hitInfo.transform.position.y - _transform.position.y;
		}

		_beacon.localScale = _beacon.localScale.SetY( normalizedHeight );
		_particleSystem.startLifetime = normalizedHeight / _particleSystem.startSpeed * 0.5f; // when the speed of a particle is 1

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
		_beaconRenderer.enabled = true;
		_particleSystem.Play();
	}
}
