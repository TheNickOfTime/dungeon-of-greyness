using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	//Components-------------------------------------------------------------------------------------------------------/
	private Animator m_Anim;
	public Animator Anim
	{
		get
		{
			return m_Anim;
		}
	}

	private Rigidbody2D m_Rig;
	
	//Values-----------------------------------------------------------------------------------------------------------/
	[SerializeField] private float m_MoveSpeed;
	public bool m_CanMove = true;

	private void Awake()
	{
		m_Anim = GetComponent<Animator>();
		m_Rig = GetComponent<Rigidbody2D>();
	}

	public void Move(Vector2 direction)
	{
		if (m_CanMove)
		{
			m_Rig.velocity = direction * m_MoveSpeed;
		}
		else
		{
			m_Rig.velocity = Vector2.zero;
		}
	}

	public void Attack()
	{
		Anim.SetTrigger("Attack");
		
	}
}
