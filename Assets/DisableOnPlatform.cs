using UnityEngine;
using System.Collections;

public class DisableOnPlatform : MonoBehaviour 
{
	[SerializeField] string platformName = "_OSX";

	void Start ()
	{
		if ( PlatformUtils.platformName == platformName )
		{
			gameObject.SetActive( false );
		}
	}
}
