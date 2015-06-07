using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	public void StartGame()
	{
		LevelUtils.LoadLevel( Level.MainGame );
	}

	public void Credits()
	{
		LevelUtils.LoadLevel( Level.Credits );
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
