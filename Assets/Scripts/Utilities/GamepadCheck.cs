﻿using UnityEngine;
using System.Collections;

public class GamepadCheck : MonoBehaviour
{
	void Update()
	{
		if ( Time.frameCount % 60 == 0 )
		{
			InputUtils.CheckForController();
		}
	}
}
