using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour, IHittable
{
	#region Components---------------------------------------------------------------------------------------------------------/
	
	private Animator m_Anim;
	private Rigidbody2D m_Rig;
	
	#endregion

	#region Values-------------------------------------------------------------------------------------------------------------/
	
	//Movement
	[Header("Movement")]
	[SerializeField] private float m_MoveSpeed = 1;
	private Vector2 m_Direction = new Vector2(0, -1);
	
	//Stats
	[Header("Stats")]
	[SerializeField] private float m_HealthMax = 5;
	private float m_HealthCurrent;
	
	[Space]
	
	[SerializeField] private float m_BaseDamage = 1;
	[SerializeField] private float m_HeavyDamage = 4;
	
	[Space]
	
	[SerializeField] private int m_ComboMax =1;
	private int m_ComboCurrent;
	
	[Space]
	
	[SerializeField] private Vector2 m_HitBoxSize = new Vector2(2, 2);
	
	//Config
	[HideInInspector] public bool m_CanMove = true;
	
	#endregion

	#region Properties---------------------------------------------------------------------------------------------------------/
	
	public Animator Anim
	{
		get
		{
			return m_Anim;
		}
	}
	
	public Rigidbody2D Rig
	{
		get
		{
			return m_Rig;
		}
	}
	
	public Vector2 Direction
	{
		get
		{
			return m_Direction;
		}
		set
		{
			m_Direction = value;
		}
	}
	
	public float HealthMax
	{
		get
		{
			return m_HealthMax;
		}
	}
	
	public float HealthCurrent
	{
		get
		{
			return m_HealthCurrent;
		}
		set
		{
			m_HealthCurrent = value;
			if (m_HealthCurrent <= 0)
			{
				OnDeath();
			}
		}
	}

	public int ComboCurrent
	{
		get
		{
			return m_ComboCurrent + 1;
			
		}
		set
		{
			m_ComboCurrent = value;
			if (m_ComboCurrent >= m_ComboMax)
			{
				m_ComboCurrent = 0;
			}
		}
	}

	#endregion

	//Monobehavior-----------------------------------------------------------------------------------------------------/
	private void Awake()
	{
		//Set component references
		m_Anim = GetComponent<Animator>();
		m_Rig = GetComponent<Rigidbody2D>();
		
		//Initialize values
		HealthCurrent = HealthMax;
	}

	//Actions----------------------------------------------------------------------------------------------------------/
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

	//Reactions--------------------------------------------------------------------------------------------------------/
	public void OnHit(Vector2 direction, float damage)
	{
		m_Anim.SetTrigger("Stun");
		HealthCurrent -= damage;
	}

	public void OnDeath()
	{
		m_Anim.SetTrigger("Die");
		Destroy(GetComponent<Collider2D>());
		Destroy(GetComponent<Rigidbody2D>());
		Destroy(GetComponent<Controller>());
		Destroy(this);
	}

	public void Knockback(Vector2 direction)
	{
		m_Rig.velocity = direction;
	}

	//Utility----------------------------------------------------------------------------------------------------------/
	
	public void AttackHitBox()
	{
		Vector2 offset = GetComponent<SpriteRenderer>().bounds.center;
		Collider2D[] hits = Physics2D.OverlapBoxAll(offset + Direction, m_HitBoxSize, 0);
		foreach (Collider2D hit in hits)
		{
			IHittable hitref = hit.gameObject.GetComponent<IHittable>();
			if (hitref != null && hit.gameObject != this.gameObject && hit.gameObject.layer != gameObject.layer)
			{
				CameraShaker.instance.Shake(1.5f * ComboCurrent, 1, 0.2f);
				hitref.OnHit(Direction, m_BaseDamage * ComboCurrent);
			}
		}
	}

	public void HeavyHitBox()
	{
		Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + Direction, new Vector2(3, 3), 0);
		foreach (Collider2D hit in hits)
		{
			IHittable hitref = hit.gameObject.GetComponent<IHittable>();
			if (hitref != null && hit.gameObject != this.gameObject && hit.gameObject.layer != gameObject.layer)
			{
				CameraShaker.instance.Shake(4, 1, 0.25f);
				hitref.OnHit(Direction, m_HeavyDamage);
			}
		}
	}
}
