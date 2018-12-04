using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeSprite : MonoBehaviour
{
	public SpriteRenderer Sprite;
	
	private Color m_StartColor = Color.magenta;
	private Color m_EndColor = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0);
	private float timer = 0.2f;

	private IEnumerator Start()
	{
		float t = 0;
		while (t < timer)
		{
			t += Time.deltaTime;
			
			Sprite.color = Color.Lerp(m_StartColor, m_EndColor, t / timer);

			yield return null;
		}
		
		Destroy(gameObject);
	}
}
