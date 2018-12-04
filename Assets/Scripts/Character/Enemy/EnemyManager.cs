using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	public static EnemyManager instance;
	
	[Range(1, 5)]
	[SerializeField] private int m_TokenCount = 1;
	
	private Transform[] m_Waves;
	private List<EnemyController> m_CurrentWave;
	public EnemyController[] CurrentWave
	{
		get { return m_CurrentWave.ToArray(); }
	}

	private int m_WaveIndex = -1;

	private bool[] m_Tokens;

	private Coroutine m_NextWaveSequence;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
		
		m_Waves = new Transform[transform.childCount];
		for (int i = 0; i < m_Waves.Length; i++)
		{
			m_Waves[i] = transform.GetChild(i);
		}
		
		m_CurrentWave = new List<EnemyController>();
	}

	private void Start()
	{
		NextWave();
	}

	private void Update()
	{
		if(m_NextWaveSequence == null && m_WaveIndex < m_Waves.Length)
		{
			CheckWave();
		}
	}

	private void NextWave()
	{
		//Setting new wave
		m_WaveIndex++;

		Transform wave = m_Waves[m_WaveIndex];
		
		wave.gameObject.SetActive(true);

		m_CurrentWave.Clear();
		for (int i = 0; i < wave.childCount; i++)
		{
			m_CurrentWave.Add(wave.GetChild(i).GetComponent<EnemyController>());
		}
		
		//Setting initial tokens
		m_Tokens = new bool[m_CurrentWave.Count];
		
		int[] tokenIndexes = new int[m_TokenCount];
		for (int i = 0; i < tokenIndexes.Length; i++)
		{
			int index = -1;
			while (index < 0 || m_Tokens[index] == true)
			{
				index = Random.Range(0, m_Tokens.Length);
			}

			m_Tokens[index] = true;
			m_CurrentWave[index].m_HasToken = true;
		}
	}

	private void CheckTokens()
	{

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

	public void TransferToken(EnemyController tokenHolder)
	{
//		StartCoroutine(TransferSequence(tokenHolder));
	}

	public void TransferToken()
	{

	}

	private IEnumerator NextWaveSequence()
	{
		if (m_WaveIndex >= m_Waves.Length -1)
		{
			m_WaveIndex++;
			UI_Gameplay.instance.FlashPanel = true;
			UI_Gameplay.instance.WavePanel.SetActive(true);
			UI_Gameplay.instance.WaveCountdown.enabled = false;
			UI_Gameplay.instance.WaveText = "Room Cleared";
			Debug.Log("WOW");
			yield break;
		}

		UI_Gameplay.instance.FlashPanel = true;
		UI_Gameplay.instance.WavePanel.SetActive(true);
		UI_Gameplay.instance.WaveText = "More Enemies Incoming";

		for (int i = 5; i >= 0; i--)
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

		UI_Gameplay.instance.WavePanel.SetActive(false);
		UI_Gameplay.instance.FlashPanel = false;
		UI_Gameplay.instance.FlashPanel = true;

		m_NextWaveSequence = null;
		NextWave();
	}

//	private IEnumerator TransferSequence(EnemyController tokenHolder)
//	{
//		int index = m_CurrentWave.IndexOf(tokenHolder);
//		m_CurrentWave[index].m_HasToken = false;
//		m_Tokens[index] = false;
//		
//		yield return new WaitForSeconds(0.75f);
//		
//		index = -1;
//		while (index < 0 || m_Tokens[index] == true)
//		{
//			index = Random.Range(0, m_Tokens.Length);
//		}
//		
//		m_Tokens[index] = true;
//		m_CurrentWave[index].m_HasToken = true;
//	}
}
