using UnityEngine;
using System.Collections;

public class Cuttable : MonoBehaviour
{
	[SerializeField] float health = 0;
	[SerializeField] ParticleSystem _particlePrefab = null;
	[SerializeField] float _particleLifetime = 0;
	[SerializeField] Color _particleColor;
	[SerializeField] float _verticalOffset = 0;
	[SerializeField] int _maxNumberOfSwipes = 0;

	public void Cut( float cuttingLevel )
	{
		if ( health / cuttingLevel <= _maxNumberOfSwipes )
		{
			health -= cuttingLevel;

			GameObject particleObj = (GameObject)Instantiate( _particlePrefab,
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
