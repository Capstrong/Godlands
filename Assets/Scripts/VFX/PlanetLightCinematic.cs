using UnityEngine;
using System.Collections;

public class PlanetLightCinematic : MonoBehaviour
{
	[Tooltip( "Rate of ratation in degrees per second. Rotation is around the y axis." )]
	[SerializeField] float _rotationSpeed = 0.1f;
	[SerializeField] float _startRotation = 0f;
	[SerializeField] float _endRotation = 0f;
	[SerializeField] float _alphaIntensity = 1f;
	[SerializeField] float _lightIntensity = 1f;

	private float _currentRotation = 0f;
	private Vector4 _lightDirection;
	private Renderer _renderer;

	void Awake()
	{
		_renderer = GetComponent<Renderer>();
		_lightDirection = Vector3.forward;
		_renderer.material.SetFloat( "_AlphaIntensity", _alphaIntensity );
		_renderer.material.SetFloat( "_LightIntensity", _lightIntensity );
		_currentRotation = _startRotation;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ( _currentRotation <= _endRotation )
		{
			_currentRotation += _rotationSpeed * Time.deltaTime;
		}

		_lightDirection = Quaternion.Euler( 0f, _currentRotation, 0f ) * Vector3.forward;
		_renderer.material.SetVector( "_LightDir", _lightDirection );
	}
}
