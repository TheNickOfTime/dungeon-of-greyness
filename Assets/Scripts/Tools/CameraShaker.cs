using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
	public static CameraShaker instance;

	private CinemachineImpulseSource m_Source;
	private Transform m_Cam;

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

		m_Source = GetComponent<CinemachineImpulseSource>();
		m_Cam = Camera.main.transform;
	}

	public void Shake(float amplitude, float frequency, float time)
	{
		transform.position = m_Cam.position;

		m_Source.m_ImpulseDefinition.m_AmplitudeGain = amplitude;
		m_Source.m_ImpulseDefinition.m_FrequencyGain = frequency;
		m_Source.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = time;
		
		m_Source.GenerateImpulse();
	}
}
