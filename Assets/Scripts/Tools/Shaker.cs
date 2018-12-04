using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using XInputDotNetPure;

public class Shaker : MonoBehaviour
{
	public static Shaker instance;

	private CinemachineImpulseSource m_Source;
	private Transform m_Cam;

	[SerializeField] private AnimationCurve m_HapticFalloff;

	private bool canCameraShake = true;

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

	public void CameraShake(float amplitude, float frequency, float time)
	{
		if(canCameraShake)
		{
			transform.position = m_Cam.position;

			m_Source.m_ImpulseDefinition.m_AmplitudeGain = amplitude;
			m_Source.m_ImpulseDefinition.m_FrequencyGain = frequency;
			m_Source.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = time;

			m_Source.GenerateImpulse();
			StartCoroutine(ShakeDelay(time));
		}
	}

	public void HapticShake(float time, float intensity)
	{
		StartCoroutine(HapticFeedback(time, intensity));
	}

	private IEnumerator ShakeDelay(float delay)
	{
		canCameraShake = false;
		yield return new WaitForSecondsRealtime(delay);
		canCameraShake = true;
	}

	private IEnumerator HapticFeedback(float time, float intensity)
	{
		float t = 0;
		while(t < time)
		{
			t += Time.deltaTime;

			float vib = intensity * m_HapticFalloff.Evaluate(t / time);
			GamePad.SetVibration(0, vib, vib);

			yield return null;
		}
	}
}
