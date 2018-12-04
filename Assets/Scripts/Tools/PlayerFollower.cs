using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerFollower : MonoBehaviour
{
	private void Update()
	{
		PlayerController player = FindObjectOfType<PlayerController>();
		
		if (player != null)
		{
			transform.position = player.transform.position;
		}
	}
}
