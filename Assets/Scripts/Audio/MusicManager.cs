using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager instance;

	private AudioSource m_Source;
	private static AudioClip m_CurrentClip;
	
	private void Awake()
	{
		if (instance != null)
		{
			m_CurrentClip = GetComponent<AudioSource>().clip;
			if (m_CurrentClip != instance.m_Source.clip)
			{
				instance.StartCoroutine(instance.FadeTracks());
			}
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			m_Source = GetComponent<AudioSource>();
			m_CurrentClip = m_Source.clip;
			DontDestroyOnLoad(gameObject);
		}
	}

	private IEnumerator FadeTracks()
	{
		float timer = 0.5f;
		float t = timer;
		while (t > 0)
		{
			t -= Time.deltaTime;

			m_Source.volume =  t / timer;
			
			yield return null;
		}

		m_Source.clip = m_CurrentClip;
		m_Source.Play();
		
		while (t < timer)
		{
			t += Time.deltaTime;

			m_Source.volume = t / timer;
			
			yield return null;
		}
	}
}
