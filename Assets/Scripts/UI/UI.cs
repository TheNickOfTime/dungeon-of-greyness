using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class UI : SerializedMonoBehaviour
{
	public void Quit()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
	
	public void MainMenu()
	{
		SceneLoader.instance.LoadLevel("MainMenu", "");
	}
}
