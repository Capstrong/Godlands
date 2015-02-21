using UnityEngine;
using System.Collections;

public class DayCycleManager : SingletonBehaviour<DayCycleManager> {

	[SerializeField,DisplayOnly]
	float _lightLevel = 0f;
	public float lightLevel
	{
		get
		{
			return _lightLevel;
		}
		private set
		{
			_lightLevel = value;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
