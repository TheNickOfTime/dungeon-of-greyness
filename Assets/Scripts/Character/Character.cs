﻿using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Cinemachine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using XInputDotNetPure;

public class Character : SerializedMonoBehaviour
{
	#region Components---------------------------------------------------------------------------------------------------------/

	private Controller m_Controller;
	private Animator m_Anim;
	private Rigidbody2D m_Rig;
	private SpriteRenderer m_Ren;
	
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
	[SerializeField]private float m_HealthCurrent;
	
	[Space]
	
	[SerializeField] private float m_BaseDamage = 1;
	[SerializeField] private float m_HeavyDamage = 4;
	
	[Space]
	
	[SerializeField] private int m_ComboMax =1;
	private int m_ComboCurrent;
	
	[Space]
	
	[SerializeField] private Vector2 m_HitBoxSize = new Vector2(2, 2);
	[SerializeField] private Vector2 m_HeavyHitBoxSize = new Vector2(3, 3);
	
	//Audio
	[Header("Audio")]
	[SerializeField] private AudioClip[] m_AttackSounds;
	[OdinSerialize] private Dictionary<string, AudioClip> m_AudioClips;

	//Config
	private bool m_CanMove = true;
	private bool m_CanRecieveDamge = true;
	
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

	public bool CanMove
	{
		get { return m_CanMove; }
		set { m_CanMove = value; }
	}

	public bool CanRecieveDamge
	{
		get
		{
			return m_CanRecieveDamge;
		}
	}

	#endregion

	#region Monobehavior-------------------------------------------------------------------------------------------------------/
	
	private void Awake()
	{
		//Set component references
		m_Controller = GetComponent<Controller>();
		m_Anim = GetComponent<Animator>();
		m_Rig = GetComponent<Rigidbody2D>();
		m_Ren = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		//Initialize values
		HealthCurrent = HealthMax;
	}
	
	#endregion

	#region Actions------------------------------------------------------------------------------------------------------------/
	
	public void Move(Vector2 direction)
	{
		if (CanMove)
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

	public void HeavyCharge()
	{
		Anim.SetTrigger("Heavy Charge");
	}

	public void HeavyAttack()
	{
		Anim.SetTrigger("Heavy Attack");
	}

	public void Dash(Vector2 direction)
	{
		Vector2 targetPos = (Vector2)transform.position + direction * m_DashDistance;
		m_Anim.SetTrigger("Dash");
		StartCoroutine(LerpMovement(targetPos, m_DashTime));
	}
	
	#endregion

	#region Reactions----------------------------------------------------------------------------------------------------------/
	
	public void Knockback(Vector2 direction)
	{
		m_Rig.velocity = direction;
	}
	
	#endregion

	#region Coroutines---------------------------------------------------------------------------------------------------------/

	private IEnumerator LerpMovement(Vector2 targetPosition, float timer)
	{
		Vector2 direction = Vector3.Normalize(targetPosition - (Vector2)transform.position);
//		Vector2 startPos = transform.position;
		
		CanMove = false;
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Water"), true);
		
		float t = 0;
		while (t < timer)
		{
			t += Time.deltaTime;

			m_Rig.velocity = direction * m_DashDistance / m_DashTime;

			yield return null;
		}
		
		CanMove = true;
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Water"), false);

		if (Physics2D.OverlapPoint(transform.position, 1 << LayerMask.NameToLayer("Water")))
		{
			m_Anim.SetTrigger("Stun Light");
			m_Anim.SetTrigger("Die");
			yield return new WaitForSeconds(1.0f);
			SceneLoader.instance.LoadLevel("","");
			yield return new WaitForSecondsRealtime(0.5f);
			m_Anim.Play("Idle");
		}
	}

	#endregion

	#region Animation Events---------------------------------------------------------------------------------------------------/
	
	public void AttackHitBox()
	{
		Vector2 offset = GetComponent<SpriteRenderer>().bounds.center;
		Collider2D[] hits = Physics2D.OverlapBoxAll(offset + Direction, m_HitBoxSize, 0);
		foreach (Collider2D hit in hits)
		{
//			if(Physics2D.LinecastAll(transform.position, hit.transform.position,  1 << LayerMask.NameToLayer("Enemy")).Length > 1)
//				return;
			
			IHittable hitref = hit.gameObject.GetComponent<IHittable>();
			Character charref = hit.gameObject.GetComponent<Character>();
			if (hitref != null && hit.gameObject != this.gameObject && hit.gameObject.layer != gameObject.layer)
			{
				if (charref != null && charref.CanRecieveDamge || charref == null)
				{
					Shaker.instance.CameraShake(1.5f * ComboCurrent, 1, 0.2f);
					hitref.OnHit(Direction, m_BaseDamage * ComboCurrent);
				
					if (m_Controller is PlayerController)
					{
						Shaker.instance.HapticShake(0.15f, 0.5f);
					}
				}
			}
		}
	}

	public void HeavyHitBox()
	{
		Vector2 offset = GetComponent<SpriteRenderer>().bounds.center;
		Collider2D[] hits = Physics2D.OverlapBoxAll(offset + Direction, m_HeavyHitBoxSize, 0);
		foreach (Collider2D hit in hits)
		{
			IHittable hitref = hit.gameObject.GetComponent<IHittable>();
			Character charref = hit.gameObject.GetComponent<Character>();
			if (hitref != null && hit.gameObject != this.gameObject && hit.gameObject.layer != gameObject.layer)
			{
				if (charref != null && charref.CanRecieveDamge || charref == null)
				{
					Shaker.instance.CameraShake(1.5f * 3, 1, 0.2f);
					hitref.OnHit(Direction, m_HeavyDamage);
				
					if (m_Controller is PlayerController)
					{
						Shaker.instance.HapticShake(0.21f, 0.65f);
					}
				}
			}
		}
	}

	public void SpawnSpriteTrail()
	{
		GameObject obj = new GameObject();
		obj.transform.position = transform.position;
		SpriteRenderer ren = obj.AddComponent<SpriteRenderer>();
		ren.sprite = m_Ren.sprite;
		FadeSprite fadeScript = obj.AddComponent<FadeSprite>();
		fadeScript.Sprite = ren;
	}

	public void MovementEnable()
	{
		CanMove = true;
	}

	public void MovementDisable()
	{
		CanMove = false;
	}

	public void PlayAttackNoise()
	{
		SFXManager.PlayClipAtPoint(m_AttackSounds[Random.Range(0, m_AttackSounds.Length)], transform.position);
	}

	public void PlayNoise(string key)
	{
		if (m_AudioClips.ContainsKey(key))
			SFXManager.PlayClipAtPoint(m_AudioClips[key], transform.position);
		else Debug.LogWarning("Key: " + key + " does not exist. Try checking your spelling, idiot!");
	}
	
	#endregion
}
