using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
	public static SceneLoader instance;

	[SerializeField] private Image m_FadePanel;

	private string m_LastSpawn;
	private string m_LastLevel;

	private float AFKTimer = 0;

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
//		PersistentData.HealthPacks = 1;
	}

	private void Update()
	{
		#if SPRING_SHOW
		AFKCheck();
		#endif
	}

	#region Load Level

	public void LoadLevel(string levelName, string spawnName)
	{
		if (levelName == "")
		{
			PlayerController.instance.HealthPacks = PersistentData.HealthPacks;
		}
		else
		{
			PersistentData.HealthPacks = PlayerController.instance.HealthPacks;
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

		yield return FadeOut();

		//Do Stuff-----------------------------------------------------------------------------------------------------/
		yield return new WaitForSecondsRealtime(1);
		yield return new WaitUntil(() => levelAsync.progress >= 0.9f);
		levelAsync.allowSceneActivation = true;

		yield return null;
		
		if (levelName != "MainMenu")
		{
			Transform spawnLocation = GameObject.Find(spawnName).transform;
			PlayerController.instance.transform.position = spawnLocation.position;
		}
		else
		{
			Destroy(PlayerController.instance.gameObject);
		}


		yield return FadeIn();
	}

	#endregion

	public IEnumerator FadeOut()
	{
		//Fade to black------------------------------------------------------------------------------------------------/
		float timer = 0.5f;
		float t = 0;
		while(t < timer)
		{
			t += Time.unscaledDeltaTime;

			m_FadePanel.color = Color.Lerp(Color.clear, Color.black, t / timer);

			yield return null;
		}
	}

	public IEnumerator FadeIn()
	{
		//Fade to scene------------------------------------------------------------------------------------------------/
		float timer = 0.5f;
		float t = 0;
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

	#region StartGame

	public void StartGame()
	{
		StartCoroutine(StartGameSequence());
	}

	private IEnumerator StartGameSequence()
	{
		Time.timeScale = 0.0f;

		AsyncOperation levelAsync = SceneManager.LoadSceneAsync("Area_00", LoadSceneMode.Single);
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

	#endregion

	private void AFKCheck()
	{
		AFKTimer += Time.deltaTime;
		
		if (Input.anyKey)
		{
			AFKTimer = 0;
		}
		else if(AFKTimer > 90.0f)
		{
			Application.Quit();
		}
	}
}
