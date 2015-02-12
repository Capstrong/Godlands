using UnityEngine;
using System.Collections;

public class CutableA : MonoBehaviour
{
	[SerializeField, Range( 0,30 )]
	int _level = 0;
	[SerializeField] GameObject _particlePrefab = null;
	[SerializeField] float _particleLifetime = 0;
	[SerializeField] float _verticalOffset = 0;

	public void Cut( float cuttingLevel )
	{
		if ( cuttingLevel >= _level )
		{
			Destroy( gameObject );
			GameObject particleObj = (GameObject)Instantiate( _particlePrefab, 
			                                                  transform.position + new Vector3(0,_verticalOffset,0),
			                                                  Quaternion.identity );
			Destroy( particleObj, _particleLifetime );
		}
	}
}
