using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour, IHittable
{
	private SpriteRenderer m_Ren;
	private Rigidbody2D m_Rig;
	
	[SerializeField] private Sprite[] m_Sprites;
	[SerializeField] private int m_Index;
	[Space] [SerializeField] private float m_MoveAmount = 1;
	[SerializeField] private GameObject m_Particles;
	[SerializeField] private Color m_FlashColor = Color.red;
	

	private void Awake()
	{
		m_Ren = GetComponent<SpriteRenderer>();
		m_Rig = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		m_Rig.velocity = Vector2.Lerp(m_Rig.velocity, Vector2.zero, Time.deltaTime * 5);
	}

	public void OnHit(Vector2 direction, float damage)
	{
		Instantiate(m_Particles, transform.position, Quaternion.identity);
		
		m_Index++;
		if (m_Index >= m_Sprites.Length - 1)
		{
			GetComponent<Collider2D>().enabled = false;
		}
		m_Rig.velocity = direction * m_MoveAmount;
		StartCoroutine(SpriteSwap());
	}

	private IEnumerator SpriteSwap()
	{
		m_Ren.sprite = m_Sprites[m_Index];
		m_Ren.color = m_FlashColor;
		yield return new WaitForSeconds(0.15f);
		
		m_Ren.color = Color.white;
	}
}
