using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosablePanel : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetButtonDown("Interact"))
		{
			gameObject.SetActive(false);
		}
	}
}
