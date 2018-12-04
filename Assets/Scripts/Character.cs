using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using XInputDotNetPure;

public class Character : MonoBehaviour
{
	#region Components---------------------------------------------------------------------------------------------------------/

	private Controller m_Controller;
	private Animator m_Anim;
	private Rigidbody2D m_Rig;
	
	#endregion

	#region Values-------------------------------------------------------------------------------------------------------------/
	
	//Movement
	[Header("Movement")]
	[SerializeField] private float m_MoveSpeed = 1;
	[SerializeField] private float m_DashDistance = 1;
	[SerializeField] private float m_DashTime = 0.1f;
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
	
	//Audio
	[Header("Audio")]
	public AudioClip[] m_AttackSounds;
	public AudioClip[] m_HitSounds;
	public AudioClip[] m_DeathSounds;
	
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
			if (m_Controller is PlayerController)
			{
				UI_Gameplay.instance.HealthBar = value / HealthMax;
			}
			if (m_HealthCurrent <= 0)
			{
				m_Controller.OnDeath();
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
		m_Controller = GetComponent<Controller>();
		m_Anim = GetComponent<Animator>();
		m_Rig = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
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

	public void Dash(Vector2 direction)
	{
		Vector2 targetPos = (Vector2)transform.position + direction * m_DashDistance;
		StartCoroutine(LerpMovement(targetPos, m_DashTime));
	}

	//Reactions--------------------------------------------------------------------------------------------------------/
	public void Knockback(Vector2 direction)
	{
		m_Rig.velocity = direction;
	}

	#region Coroutines

	private IEnumerator LerpMovement(Vector2 targetPosition, float timer)
	{
		Vector2 startPos = transform.position;

		m_CanMove = false;
		
		float t = 0;
		while (t < timer)
		{
			t += Time.deltaTime;

			m_Rig.MovePosition(Vector3.Lerp(startPos, targetPosition, t / timer));

			yield return null;
		}

		m_CanMove = true;
	}

	#endregion

	#region Utility------------------------------------------------------------------------------------------------------------/
	
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
				
				if (m_Controller is PlayerController)
				{
					StartCoroutine((m_Controller as PlayerController).HapticFeedback(0.1f, 0.5f));
				}
			}
		}
		PlayAttackNoise();
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

	public void PlayAttackNoise()
	{
		AudioSource.PlayClipAtPoint(m_AttackSounds[Random.RandomRange(0, m_AttackSounds.Length)], transform.position);
	}

	public void PlayHitNoise()
	{
		AudioSource.PlayClipAtPoint(m_HitSounds[Random.RandomRange(0, m_HitSounds.Length)], transform.position);
	}

	public void PlayDeathNoise()
	{
		AudioSource.PlayClipAtPoint(m_DeathSounds[Random.RandomRange(0, m_DeathSounds.Length)], transform.position);
	}
	
	#endregion
}
