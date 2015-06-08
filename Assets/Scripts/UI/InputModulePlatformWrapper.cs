using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputModulePlatformWrapper : MonoBehaviour
{
	void Start () 
	{
		StandaloneInputModule inputModule = GetComponent<StandaloneInputModule>();

		inputModule.verticalAxis += PlatformUtils.platformName;
		inputModule.horizontalAxis += PlatformUtils.platformName;
	}
}
