using UnityEngine;
using System.Collections;

public class ResourceHolder : MonoBehaviour
{
	public GameObject resource;

	[SerializeField] float resourceHeightOffset = 0.17f;
	[SerializeField] float _maxHeight = 12f;

	Transform _transform;

	void Awake()
	{
		_transform = GetComponent<Transform>();

		RaycastHit hitInfo;

		Physics.Raycast( new Ray( _transform.position, Vector3.up), out hitInfo, _maxHeight );

		if ( hitInfo.transform )
		{
			// The .5 is to correct for some scale thing
			float normalizedHeight = ( hitInfo.transform.position.y - _transform.position.y ) / ( _maxHeight * 0.5f );
			_transform.localScale = _transform.localScale.SetY( normalizedHeight );

			ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
			particleSystem.startLifetime *= normalizedHeight;
		}

		resource = WadeUtils.Instantiate( resource, Vector3.up * resourceHeightOffset, Quaternion.identity );
		resource.GetComponent<Transform>().SetParent( _transform, false );
		resource.GetComponent<InventoryItem>().beaconObj = GetComponentInChildren<BeaconTag>().gameObject;
	}
}
