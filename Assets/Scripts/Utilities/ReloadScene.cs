using UnityEngine;
using System.Collections;

public class ReloadScene : MonoBehaviour
{	
	// Update is called once per frame
	void Update () {

		if ( Input.GetKeyDown( KeyCode.BackQuote ) )
		{
			Application.LoadLevel( Application.loadedLevel );
		}
	}
}
