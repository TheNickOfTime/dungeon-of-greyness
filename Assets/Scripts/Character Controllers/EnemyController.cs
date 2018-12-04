using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Controller
{
	#region Components---------------------------------------------------------------------------------------------------------/
	
	private Character m_Char;
	private SpriteRenderer m_Ren;
	
	[SerializeField] private GameObject m_Particles;

	#endregion

	#region State--------------------------------------------------------------------------------------------------------------/

	private enum EnemyState
	{
		Idle,
		Alert,
		Attack
	}

	private EnemyState m_State = EnemyState.Idle;
	private EnemyState State
	{
		get
		{
			return m_State;
		}
		set
		{
			m_State = value;
			
			switch (value)
			{
				case EnemyState.Idle:
					m_Char.Anim.SetBool("IsMoving", false);
					break;
				case EnemyState.Alert:
					m_Char.Anim.SetBool("IsMoving", true);
					break;
				case EnemyState.Attack:
					m_Char.Anim.SetTrigger("Attack");
					break;
			}
		}
	}

	#endregion
	
	#region Values-------------------------------------------------------------------------------------------------------------/

	[SerializeField] private float m_AlertRadius = 5;
	[SerializeField] private float m_AttackRadius = 1;
	
	private Transform m_Target;
	
	#endregion

	protected void Awake()
	{
		//Assign component references
		m_Char = GetComponent<Character>();
		m_Ren = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		//Set Values
		m_Target = PlayerController.instance.transform;
	}

	private void Update()
	{
		switch (State)
		{
			case EnemyState.Idle:
				if (PlayerInRadius(m_AlertRadius))
				{
					State = EnemyState.Alert;
					m_Char.Move(Vector2.zero);
				}
				break;
			
			case EnemyState.Alert:
				//Checks if player has left enemy's range
				if (!PlayerInRadius(m_AlertRadius + 1))
				{
					State = EnemyState.Idle;
					return;
				}
				
				//Checks if the player can be attacked;
				if (PlayerInRadius(m_AttackRadius))
				{
					State = EnemyState.Attack;
				}

				//Applies movement
				Vector2 direction = Vector3.Normalize(m_Target.position - transform.position);
				m_Char.Direction = direction;
				
				m_Char.Anim.SetFloat("Horizontal", direction.x < 0 ? -1 : 1);
				m_Char.Move(direction);
				break;
			
			case EnemyState.Attack:
				m_Char.Move(Vector2.zero);
				
				//If the player is no longer in the attack state, returns to alert state
				if (!m_Char.Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
				{
					m_State = EnemyState.Alert;
				}
				break;
		}
	}

	private bool PlayerInRadius(float radius)
	{
		Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, 1 << LayerMask.NameToLayer("Player"));
		return hit != null;
	}
}
