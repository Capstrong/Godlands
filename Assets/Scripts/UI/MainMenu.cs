using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	public void StartGame()
	{
		LevelUtils.LoadLevel( Level.MainGame );
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
