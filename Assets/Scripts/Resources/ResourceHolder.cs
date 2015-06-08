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

	// Only called by the resource spawner so the resource holder should only have resources in it, not eggs
	public void Initialize( ResourceSpawner spawner )
	{
		transform.parent = spawner.transform;

		ResourceItem resourceItem = resource.GetComponent<ResourceItem>();

		if ( resourceItem )
		{
			resourceItem.Initialize( spawner );
		}
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
