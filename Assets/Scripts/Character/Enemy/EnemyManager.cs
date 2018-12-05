using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
	//Singleton--------------------------------------------------------------------------------------------------------/
	public static EnemyManager instance;

	//Tokens-----------------------------------------------------------------------------------------------------------/
	[Range(1, 5)]
	[SerializeField] private int m_TokenCount = 1;
	
	//Waves------------------------------------------------------------------------------------------------------------/
	private Transform[] m_Waves;
	private List<EnemyController> m_CurrentWave;
	private EnemyController[] m_NextWave;
	public List<EnemyController> CurrentWave
	{
		get { return m_CurrentWave; }
	}
	private int m_WaveIndex = -1;

	private bool[] m_Tokens;

	//Misc-------------------------------------------------------------------------------------------------------------/
	[SerializeField] private GameObject m_EnemySpawnMarker;

	private UnityEvent m_FinishWavesEvent;
	
	private Coroutine m_NextWaveSequence;
	
	//Constants--------------------------------------------------------------------------------------------------------/
	private const int SecondsBetweenWaves = 3;


	private void Awake()
	{
		//Singleton setup
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
		
		//Initialize Waves
		m_Waves = new Transform[transform.childCount];
		for (int i = 0; i < m_Waves.Length; i++)
		{
			m_Waves[i] = transform.GetChild(i);
			m_Waves[i].gameObject.SetActive(i == 0);
		}
		
		//Initialize CurrentWave and NextWave
		m_CurrentWave = new List<EnemyController>();
		m_NextWave = new EnemyController[m_Waves[0].childCount];
		for (int i = 0; i < m_NextWave.Length; i++)
		{
			m_NextWave[i] = m_Waves[0].GetChild(i).GetComponent<EnemyController>();
		}
	}

	private void Start()
	{
		NextWave();
	}

	private void Update()
	{
		if(m_NextWaveSequence == null)
		{
			CheckWave();
		}
	}

	private void NextWave()
	{
		m_WaveIndex++;

		//SETTING NEW WAVE
		Transform newWave = m_Waves[m_WaveIndex];
		newWave.gameObject.SetActive(true);
		m_CurrentWave = m_NextWave.ToList();
		
		//SETTING NEXT WAVE
		if (m_Waves.Length <= m_WaveIndex + 1)
		{
			m_NextWave = new EnemyController[0];
			return;
		}
		
		Transform nextWave = m_Waves[m_WaveIndex + 1];
		m_NextWave = new EnemyController[nextWave.childCount];
		for (int i = 0; i < m_NextWave.Length; i++)
		{
			m_NextWave[i] = nextWave.GetChild(i).GetComponent<EnemyController>();
		}
	}

	public void CheckWave()
	{
		int deathToll = 0;
		for (int i = 0; i < m_CurrentWave.Count; i++)
		{
			deathToll += m_CurrentWave[i] != null ? 0 : 1;
		}

		if(deathToll == m_CurrentWave.Count)
		{
			m_NextWaveSequence = StartCoroutine(NextWaveSequence());
		}
	}

	private IEnumerator NextWaveSequence()
	{
		//Separate behavior if the last wave is cleared
		if (m_NextWave.Length == 0)
		{
			m_WaveIndex++;
			UI_Gameplay.instance.FlashPanel = true;
			UI_Gameplay.instance.WavePanel.SetActive(true);
			UI_Gameplay.instance.WaveCountdown.enabled = false;
			UI_Gameplay.instance.WaveText = "Room Cleared";

			m_FinishWavesEvent?.Invoke();

			yield break;
		}

		//Flashes the screen
		UI_Gameplay.instance.FlashPanel = true;
		UI_Gameplay.instance.WavePanel.SetActive(true);
		UI_Gameplay.instance.WaveText = "More Enemies Incoming";
		
		//Spawns markers
		GameObject[] markers = new GameObject[m_NextWave.Length];
		for (int i = 0; i < markers.Length; i++)
		{
			markers[i] = Instantiate(m_EnemySpawnMarker, m_NextWave[i].transform.position, Quaternion.identity);
		}

		//Fades numbers in countdown
		for (int i = SecondsBetweenWaves; i >= 0; i--)
		{
			float timer = 0;
			while(timer < 1)
			{
				UI_Gameplay.instance.WaveCountdown.text = i.ToString();

				timer += Time.deltaTime;

				UI_Gameplay.instance.WaveCountdown.color = Color.Lerp(Color.white, new Color(1,1,1,.5f), timer);
				yield return null;
			}
			yield return null;
		}

		//Flashes screen again
		UI_Gameplay.instance.WavePanel.SetActive(false);
		UI_Gameplay.instance.FlashPanel = false;
		UI_Gameplay.instance.FlashPanel = true;
		
		//Destroys markers
		for (int i = 0; i < markers.Length; i++)
		{
			Destroy(markers[i]);
		}

		//Starts next wave
		m_NextWaveSequence = null;
		NextWave();
	}
}
