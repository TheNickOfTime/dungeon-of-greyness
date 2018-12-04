using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	public static EnemyManager instance;
	
	[Range(1, 5)]
	[SerializeField] private int m_TokenCount = 1;
	
	private Transform[] m_Waves;
	private EnemyController[] m_CurrentWave;
	public EnemyController[] CurrentWave
	{
		get { return m_CurrentWave; }
	}

	private int m_WaveIndex = -1;

	[SerializeField] private bool[] m_Tokens;

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
	}

	private void Start()
	{
		NextWave();
	}

	private void NextWave()
	{
		//Setting new wave
		if(m_WaveIndex >= 0)
			m_Waves[m_WaveIndex].gameObject.SetActive(false);

		m_WaveIndex++;
		m_Waves[m_WaveIndex].gameObject.SetActive(true);

		m_CurrentWave = new EnemyController[m_Waves[m_WaveIndex].childCount];
		for (int i = 0; i < m_CurrentWave.Length; i++)
		{
			m_CurrentWave[i] = GetComponent<EnemyController>();
		}
		
		//Setting initial tokens
		m_Tokens = new bool[m_CurrentWave.Length];
		
		int[] tokenIndexes = new int[m_TokenCount];
		for (int i = 0; i < tokenIndexes.Length; i++)
		{
			int index = -1;
			while (index < 0 || m_Tokens[index] == true)
			{
				index = Random.Range(0, m_Tokens.Length);
			}

			m_Tokens[index] = true;
		}
	}

	public void TransferToken()
	{
		
	}
}
