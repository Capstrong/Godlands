using UnityEngine;
using System.Collections;

public class ParachuteControl : MonoBehaviour 
{
	Transform _transform = null;

	Vector3 _enabledScale = new Vector3( 1f, 1f, 1f );
	Vector3 _disabledScale = new Vector3( 0f, 0f, 0f );

	[SerializeField] float _scaleTime = 0.3f;

	void Awake()
	{
		_transform = GetComponent<Transform>();
	}

	public void SetParachuteEnabled( bool setEnabled )
	{
		StartCoroutine( SetParachuteEnabledRoutine( setEnabled ) );
	}

	IEnumerator SetParachuteEnabledRoutine( bool setEnabled )
	{
		Vector3 startScale = _transform.localScale;
		Vector3 endScale = setEnabled ? _enabledScale : _disabledScale;

		float scaleTimer = 0f;
		while( scaleTimer < _scaleTime )
		{
			_transform.localScale = Vector3.Lerp( startScale, endScale, scaleTimer/_scaleTime );

			scaleTimer += Time.deltaTime;
			yield return 0;
		}

		_transform.localScale = endScale;
	}
}
