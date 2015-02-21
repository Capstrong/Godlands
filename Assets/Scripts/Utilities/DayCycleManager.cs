using UnityEngine;
using System.Collections;

public class DayCycleManager : SingletonBehaviour<DayCycleManager> 
{
	[Tooltip( "The length (in seconds) of a day" )]
	[SerializeField] float _dayCycleLength = 60;
	public float dayCycleLength
	{
		get
		{
			return _dayCycleLength;
		}
	}

	[DisplayOnly]
	[SerializeField] float _dayCycleTimer = 0f;
	public float dayCycleTimer
	{
		get
		{
			return _dayCycleTimer;
		}
		set
		{
			_dayCycleTimer = value;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		_dayCycleTimer += Time.deltaTime;

		if ( _dayCycleTimer > _dayCycleLength )
		{
			_dayCycleTimer -= _dayCycleLength;
		}
	}
}
