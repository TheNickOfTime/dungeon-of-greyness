using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_KillAllWaves : Objective
{
	private Transform[] m_Waves;
	private EnemyController[] m_CurrentWave;

	private int m_WaveIndex = -1;

	private void Awake()
	{
		m_Waves = new Transform[transform.childCount];
		for (int i = 0; i < m_Waves.Length; i++)
		{
			m_Waves[i] = m_Waves[i];
		}
	}

	private void Start()
	{
		NextWave();
	}

	protected override bool Condition()
	{
		if(m_WaveIndex < transform.childCount)
		{
			for (int i = 0; i < m_CurrentWave.Length; i++)
			{
				if (m_CurrentWave[i] == null)
				break;
			}

			NextWave();
		}

		return m_WaveIndex >=transform.childCount;
	}

	private void NextWave()
	{
		if(m_WaveIndex >= 0)
		m_Waves[m_WaveIndex].gameObject.SetActive(false);

		m_WaveIndex++;
		m_Waves[m_WaveIndex].gameObject.SetActive(true);

		m_CurrentWave = new EnemyController[m_Waves[m_WaveIndex].childCount];
		for (int i = 0; i < m_CurrentWave.Length; i++)
		{
			m_CurrentWave[i] = GetComponent<EnemyController>();
		}
	}
}
