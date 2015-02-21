using UnityEngine;
using System.Collections;

public class DayCycleManager : SingletonBehaviour<DayCycleManager> {

	[DisplayOnly,Tooltip("0 is midnight. 1 is noon.")]
	[SerializeField] float _daylightIntensity = 1.0f;
	public float daylightIntensity
	{
		get
		{
			return _daylightIntensity;
		}
		private set
		{
			_daylightIntensity = value;
		}
	}

	[Tooltip( "The length (in seconds) of a day" )]
	[SerializeField] float _dayCycleLength = 60;
	[DisplayOnly]
	[SerializeField] float _dayCycleTimer = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		_dayCycleTimer += Time.deltaTime;

		daylightIntensity = Mathf.Cos( _dayCycleTimer / _dayCycleLength * 2 * Mathf.PI ) * 0.5f + 0.5f;

		if ( _dayCycleTimer > _dayCycleLength )
		{
			_dayCycleTimer -= _dayCycleLength;
		}
	}
}
