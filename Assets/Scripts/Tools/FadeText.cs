using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class FadeText : MonoBehaviour
{
	[SerializeField] private float m_FadeTime = 1;

	private Text m_Text;

	private void Awake()
	{
		m_Text = GetComponent<Text>();
	}

	public void Fade()
	{
		StartCoroutine(FadeSequence());
	}

	private IEnumerator FadeSequence()
	{
		Color startColor = m_Text.color;
		Color endColor = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, 0);
		
		float timer = 0;
		while (timer < m_FadeTime)
		{
			timer += Time.deltaTime;

			m_Text.color = Color.Lerp(startColor, endColor, timer / m_FadeTime);

			yield return null;
		}
		
		gameObject.SetActive(false);
	}
}
