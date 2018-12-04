using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Controller
{
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

	private bool m_DefaultRenFlip;
	
	#endregion

	#region MonoBehaviour------------------------------------------------------------------------------------------------------/
	
	private void Start()
	{
		//Set Values
		m_Target = PlayerController.instance.transform;
		m_DefaultRenFlip = m_Ren.flipX;
	}

	private void Update()
	{
		StateMachine();
	}
	
	#endregion

	#region Methods------------------------------------------------------------------------------------------------------------/

	private void StateMachine()
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

				m_Ren.flipX = direction.x < 0 ? m_DefaultRenFlip : !m_DefaultRenFlip;
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
	
	//Overrides--------------------------------------------------------------------------------------------------------/
	public override void OnDeath()
	{
		base.OnDeath();
		
		Destroy(GetComponent<Collider2D>());
		Destroy(GetComponent<Rigidbody2D>());
		Destroy(GetComponent<Controller>());
		Destroy(this);
	}

	//Utility----------------------------------------------------------------------------------------------------------/
	private bool PlayerInRadius(float radius)
	{
		Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, 1 << LayerMask.NameToLayer("Player"));
		return hit != null;
	}

	#endregion
}
