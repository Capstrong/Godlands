using UnityEngine;
using System.Collections;

public class ResourceHolder : MonoBehaviour
{
	public GameObject resource;
	private ParticleSystem[] _resourceParticles = null;
	private MeshRenderer _resourceMesh = null;

	[SerializeField] float _resourceHeightOffset = 0.17f;

	Transform _transform;

	void Awake()
	{
		_transform = GetComponent<Transform>();

		resource = WadeUtils.Instantiate( resource, Vector3.up * _resourceHeightOffset, Quaternion.identity );
		resource.GetComponent<Transform>().SetParent( _transform, false );

		_resourceParticles = resource.GetComponentsInChildren<ParticleSystem>( true );
		_resourceMesh = resource.GetComponent<MeshRenderer>();

		resource.GetComponent<InventoryPickupItem>().Initialize( this );
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
