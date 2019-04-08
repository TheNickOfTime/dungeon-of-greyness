using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class InteractText : MonoBehaviour
{
	[SerializeField] private string m_ControllerText = "rb";
	[SerializeField] private string m_KeyboardText = "e";
	
	private void OnEnable()
	{
		string prompt = "";
		if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0].Contains("Controller"))
		{
			prompt = m_ControllerText;
		}
		else
		{
			prompt = m_KeyboardText;
		}

		GetComponent<Text>().text = prompt;
	}
}
