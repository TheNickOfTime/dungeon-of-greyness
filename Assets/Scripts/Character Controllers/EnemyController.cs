using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Controller
{
	#region State--------------------------------------------------------------------------------------------------------------/

	protected enum EnemyState
	{
		Idle,
		Alert,
		Attack
	}

	[Header("Enemy Controller")]
	[SerializeField] protected EnemyState m_State = EnemyState.Idle;
	protected EnemyState State
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
					m_Char.m_CanMove = false;
					m_Char.Rig.velocity = Vector2.zero;
					break;
				case EnemyState.Alert:
					m_Char.Anim.SetBool("IsMoving", true);
					m_Char.m_CanMove = true;
					break;
				case EnemyState.Attack:
					m_Char.Anim.SetTrigger("Attack");
					break;
			}
		}
	}

	#endregion
	
	#region Values-------------------------------------------------------------------------------------------------------------/

	[SerializeField] protected float m_AlertRadius = 5;
	[SerializeField] protected float m_AttackRadius = 1;
	
	protected Transform m_Target;

	protected bool m_DefaultRenFlip;
	
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
				IdleState();
				break;
			
			case EnemyState.Alert:
				AlertState();
				break;
			
			case EnemyState.Attack:
				AttackState();
				break;
		}
	}

	protected virtual void IdleState()
	{
		if (PlayerInRadius(m_AlertRadius) && PlayerInSight())
		{
			State = EnemyState.Alert;
		}
	}

	protected virtual void AlertState()
	{
		//Checks if player has left enemy's range
		if (!PlayerInSight())
		{
			State = EnemyState.Idle;
			return;
		}
				
		//Checks if the player can be attacked;
		if (PlayerInRadius(m_AttackRadius) && PlayerInSight())
		{
			State = EnemyState.Attack;
		}

		//Applies movement
		if(m_Char.Anim.GetCurrentAnimatorStateInfo(0).IsName("Move"))
		{
			Vector2 direction = Vector3.Normalize(m_Target.position - transform.position);
			m_Char.Direction = direction;

			m_Char.Move(direction);

			m_Ren.flipX = direction.x < 0 ? m_DefaultRenFlip : !m_DefaultRenFlip;
		}
	}

	protected virtual void AttackState()
	{
		m_Char.Move(Vector2.zero);
				
		//If the player is no longer in the attack state, returns to alert state
		if (!m_Char.Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
		{
			m_State = EnemyState.Alert;
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
	protected bool PlayerInRadius(float radius)
	{
		Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, 1 << LayerMask.NameToLayer("Player"));
		return hit != null;
	}

	protected bool PlayerInSight()
	{
		bool didHit = Physics2D.Linecast(transform.position, m_Target.position, 1 << LayerMask.NameToLayer("Environment"));
		return !didHit;
	}

	#endregion
}
