using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimAwake : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<Animation>().Play();
	}
}
