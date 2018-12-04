using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractText : MonoBehaviour
{
	private void OnEnable()
	{
		string prompt = "";
		if (Input.GetJoystickNames()[0].Contains("Controller"))
		{
			prompt = "rb";
		}
		else
		{
			prompt = "e";
		}

		GetComponent<Text>().text = prompt;
	}
}
