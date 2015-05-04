using UnityEngine;
using System.Collections;

public class ResourceHolder : MonoBehaviour
{
	public GameObject resource;
	private ParticleSystem[] _resourceParticles = null;
	private MeshRenderer _resourceMesh = null;

	[SerializeField] float _resourceHeightOffset = 0.17f;
	[SerializeField] float _maxHeight = 12f;
	[SerializeField] LayerMask _collisionLayer = -1;

	Transform _transform;

	void Awake()
	{
		_transform = GetComponent<Transform>();

		RaycastHit hitInfo;

		Physics.Raycast( new Ray( _transform.position, Vector3.up ), out hitInfo, _maxHeight, _collisionLayer );

		float actualHeight = _maxHeight;

		if ( hitInfo.transform )
		{
			actualHeight = hitInfo.transform.position.y - _transform.position.y;
		}

		resource = WadeUtils.Instantiate( resource, Vector3.up * _resourceHeightOffset, Quaternion.identity );
		resource.GetComponent<Transform>().SetParent( _transform, false );

		_resourceParticles = resource.GetComponentsInChildren<ParticleSystem>( true );
		_resourceMesh = resource.GetComponent<MeshRenderer>();

		resource.GetComponent<InventoryItem>().Initialize( this );
	}

	public void Disable()
	{
		_resourceMesh.enabled = false;
		foreach ( ParticleSystem particles in _resourceParticles )
		{
			particles.Stop();
		}
	}

	public void Enable()
	{
		_resourceMesh.enabled = true;
		foreach ( ParticleSystem particles in _resourceParticles )
		{
			particles.Play();
		}
	}
}
