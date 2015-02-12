using UnityEngine;
using System.Collections;

public class CutableC : MonoBehaviour
{
	[SerializeField, Range( 0, 30 )]
	float _deathTimeRatio = 0;
	[SerializeField] GameObject _particlePrefab = null;
	[SerializeField] float _particleLifetime = 0;
	[SerializeField] float _verticalOffset = 0;

	public void Cut( float cuttingLevel )
	{
		GameObject particleObj = (GameObject)Instantiate( _particlePrefab,
		                                                  transform.position + new Vector3( 0, _verticalOffset, 0 ),
		                                                  Quaternion.identity );
		Destroy( particleObj, _particleLifetime );

		Invoke( "Reactivate", cuttingLevel * _deathTimeRatio );

		gameObject.SetActive( false );
	}

	public void Reactivate()
	{
		gameObject.SetActive( true );
	}
}
