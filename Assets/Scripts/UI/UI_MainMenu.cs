using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MainMenu : UI
{
	public void Play()
	{
		SceneLoader.instance.StartGame();
	}
}
