using UnityEngine;
using System.Collections;

public class ResourceHolder : MonoBehaviour
{
	public GameObject resource;

	[SerializeField] float resourceHeightOffset = 0.17f;
	[SerializeField] float _maxHeight = 12f;

	Transform _transform;

	Renderer _beaconRenderer = null;
	ParticleSystem _particleSystem = null;

	void Awake()
	{
		_transform = GetComponent<Transform>();
		_beaconRenderer = GetComponentInChildren<BeaconTag>().gameObject.GetComponent<Renderer>();
		_particleSystem = GetComponentInChildren<ParticleSystem>();

		RaycastHit hitInfo;

		Physics.Raycast( new Ray( _transform.position, Vector3.up), out hitInfo, _maxHeight );

		if ( hitInfo.transform )
		{
			// The .5 is to correct for some scale thing
			float normalizedHeight = ( hitInfo.transform.position.y - _transform.position.y ) / ( _maxHeight * 0.5f );
			_transform.localScale = _transform.localScale.SetY( normalizedHeight );
			_particleSystem.startLifetime *= normalizedHeight;
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
		_beaconRenderer.enabled = true;
		_particleSystem.Play();
	}
}
