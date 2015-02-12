using UnityEngine;
using System.Collections;

public class CutableB : MonoBehaviour
{
	[SerializeField, Range( 0,30 )]
	float health = 0;
	[SerializeField] GameObject _particlePrefab = null;
	[SerializeField] float _particleLifetime = 0;
	[SerializeField] float _verticalOffset = 0;

	float magicRatioNumber = 10.0f;

	public void Cut( float cuttingLevel )
	{
		if ( health / cuttingLevel <= magicRatioNumber )
		{
			health -= cuttingLevel;

			GameObject particleObj = (GameObject) Instantiate( _particlePrefab,
															  transform.position + new Vector3( 0, _verticalOffset, 0 ),
															  Quaternion.identity );
			Destroy( particleObj, _particleLifetime );

			if ( health <= 0 )
			{
				Destroy( gameObject );
			}
		}
	}
}
