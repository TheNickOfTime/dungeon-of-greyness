using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
	[SerializeField] private string m_LevelToLoad;
	[SerializeField] private string m_SpawnName;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.GetComponent<PlayerController>() != null)
		{
			SceneLoader.instance.LoadLevel(m_LevelToLoad, m_SpawnName);
		}
	}
}
