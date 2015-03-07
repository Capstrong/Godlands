using UnityEngine;
using System.Collections;

public class LoadGameOnButton : MonoBehaviour
{
	[SerializeField] string _buttonName = "";
	[SerializeField] Level _level;
	
	Button _button;

	void Start()
	{
		_button = new Button( _buttonName );
	}

	void Update()
	{
		_button.Update();

		if ( _button )
		{
			gameObject.SetActive( false ); // Immediately show a black screen. The actual level may take a bit to load
			LevelUtils.LoadLevel( _level );
		}
	}
}
