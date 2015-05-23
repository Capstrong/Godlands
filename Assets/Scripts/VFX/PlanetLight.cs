using UnityEngine;
using System.Collections;

public class PlanetLight : MonoBehaviour
{
	[Tooltip( "Rate of ratation in degrees per second. Rotation is around the y axis." )]
	[SerializeField] float _rotationSpeed = 0.1f;
	[SerializeField] MinMaxF _alphaRange;
	[SerializeField] MinMaxF _lightIntensityRange;

	private Vector4 _lightDirection;
	private Renderer _renderer;

	void Awake()
	{
		_renderer = GetComponent<Renderer>();
		_lightDirection = _renderer.material.GetVector("_LightDir");
	}

	void Update()
	{
		// rotate light direction
		Quaternion rotation = Quaternion.Euler( 0.0f, _rotationSpeed * Time.deltaTime, 0.0f );
		_lightDirection = rotation * _lightDirection;
		_renderer.material.SetVector( "_LightDir", _lightDirection );

//		// set min alpha
//		float alphaIntensity =
//			RenderSettingsManager.daylightIntensity
//			* _alphaRange.range
//			+ _alphaRange.min;
//		_renderer.material.SetFloat( "_AlphaIntensity", alphaIntensity );
//
//		// set light intensity
//		float lightIntensity =
//			RenderSettingsManager.daylightIntensity
//			* _lightIntensityRange.range
//			+ _lightIntensityRange.min;
//		_renderer.material.SetFloat( "_LightIntensity", lightIntensity );
	}
}
