﻿using UnityEngine;
using System.Collections;

public class CutableA : MonoBehaviour
{
	[SerializeField, Range( 0,30 )]
	float _level = 0;

	void Start ()
	{
	
	}
	
	void Update () 
	{
	
	}

	public void Cut( float cuttingLevel )
	{
		if ( cuttingLevel >= _level )
		{
			Destroy( gameObject );
		}
	}
}
