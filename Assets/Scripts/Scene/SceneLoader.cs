using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
	public static SceneLoader instance;

	[SerializeField] private Image m_FadePanel;
	
	private struct PlayerRespawnData
	{
		public int healthPacks;
	}
	private PlayerRespawnData m_PlayerRespawnData;

	private string m_LastSpawn;
	private string m_LastLevel;

	private void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		m_LastLevel = SceneManager.GetActiveScene().name;
		m_LastSpawn = "Player Spawn";
	}

	public void LoadLevel(string levelName, string spawnName)
	{
		if (levelName == "")
		{
			PlayerController.instance.HealthPacks = m_PlayerRespawnData.healthPacks;
		}
		
		levelName = levelName == "" ? m_LastLevel : levelName;
		m_LastLevel = levelName;

		spawnName = spawnName == "" ? m_LastSpawn : spawnName;
		m_LastSpawn = spawnName;
		
		StartCoroutine(LoadingSequence(levelName, spawnName));
	}

	private IEnumerator LoadingSequence(string levelName, string spawnName)
	{
		Time.timeScale = 0.0f;

		AsyncOperation levelAsync = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
		levelAsync.allowSceneActivation = false;

		//Fade to black------------------------------------------------------------------------------------------------/
		float timer = 0.5f;
		float t = 0;
		while(t < timer)
		{
			t += Time.unscaledDeltaTime;

			m_FadePanel.color = Color.Lerp(Color.clear, Color.black, t / timer);

			yield return null;
		}

		//Do Stuff-----------------------------------------------------------------------------------------------------/
		yield return new WaitForSecondsRealtime(1);
		yield return new WaitUntil(() => levelAsync.progress >= 0.9f);
		levelAsync.allowSceneActivation = true;
		
//		string oldSceneName = SceneManager.GetActiveScene().name;
//		SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));

		yield return null;

		Transform spawnLocation = GameObject.Find(spawnName).transform;
//		Transform player = GameObject.FindObjectOfType<PlayerController>().transform;

		PlayerController.instance.transform.position = spawnLocation.position;

		//Fade to scene------------------------------------------------------------------------------------------------/
		t = 0;
		while (t < timer)
		{
			t += Time.unscaledDeltaTime;
			m_FadePanel.color = Color.Lerp(Color.black, Color.clear, t / timer);
			if(t > 0.5f)
			{
				Time.timeScale = 1.0f;
			}

			yield return null;
		}
	}
}
