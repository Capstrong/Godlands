using UnityEngine;
using System.Collections;

public class MainMenuCursor : MonoBehaviour {

	// Use this for initialization
	void Start()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
}
