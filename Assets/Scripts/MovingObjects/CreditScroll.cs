using UnityEngine;
using System.Collections;

public class CreditScroll : Translater 
{
	void OnDestroy()
	{
		LevelUtils.LoadLevel( Level.MainMenu );
	}
}
