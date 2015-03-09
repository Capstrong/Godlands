using UnityEngine;
using System.Collections;

public class LoadSceneOnButton : MonoBehaviour
{
	[SerializeField] Level _level;

	// To be called from the UI event system
	public void LoadLevel()
	{
		gameObject.SetActive( false ); // Immediately show a black screen. The actual level may take a bit to load
		LevelUtils.LoadLevel( _level );
	}
}
