using UnityEngine;
using System.Collections;

public class PlanetLight : MonoBehaviour
{
	[Tooltip( "Rate of ratation in degrees per second. Rotation is around the y axis." )]
	[SerializeField] float _rotationSpeed = 0.1f;

	private Vector4 _lightDirection;
	private Renderer _renderer;

	void Awake()
	{
		_renderer = GetComponent<Renderer>();
		_lightDirection = _renderer.material.GetVector("_LightDir");
	}

	void Update()
	{
		Quaternion rotation = Quaternion.Euler( 0.0f, _rotationSpeed * Time.deltaTime, 0.0f );
		_lightDirection = rotation * _lightDirection;
		_renderer.material.SetVector(
			"_LightDir",
			new Vector4( _lightDirection.x, _lightDirection.y, _lightDirection.z ) );
	}
}
