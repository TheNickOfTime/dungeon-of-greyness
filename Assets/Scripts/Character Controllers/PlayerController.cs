﻿using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : Controller
{
	//Singleton--------------------------------------------------------------------------------------------------------/
	public static PlayerController instance;
	
	//Values-----------------------------------------------------------------------------------------------------------/
	private float m_TempHealth;
	
	private Vector2 m_LastDir = new Vector2(0, -1);
	
	//Properties-------------------------------------------------------------------------------------------------------/
	public float TimeScale
	{
		set { Time.timeScale = value; }
	}

	#region Monobehaviour------------------------------------------------------------------------------------------------------/
	
    private void Awake()
    {
	    if (instance != null)
	    {
		    Destroy(gameObject);
	    }
	    else
	    {
		    instance = this;
	    }
	    
        m_Char = GetComponent<Character>();
    }

	private void Update()
	{
		PlayerInput();
	}
	
	#endregion

	#region Methods------------------------------------------------------------------------------------------------------------/
		
	private void PlayerInput()
	{
		#region Movement

		//Get Values
		float h = Input.GetButtonDown("Vertical") ? 0 : Input.GetAxis("Horizontal");
		float v = Input.GetButtonDown("Horizontal") ? 0 : Input.GetAxis("Vertical");
		bool isMoving = h != 0 || v != 0;
		
		//Track Values
		if (isMoving)
		{
			Vector2 dir = Mathf.Abs(h) > Mathf.Abs(v) ? new Vector2(h, 0) : new Vector2(0, v);
			dir = Vector3.Normalize(dir);
			m_Char.Direction = dir;
		}
		
		//Use Values
		m_Char.Move(Vector3.Normalize(new Vector2(h, v)));
		
		m_Char.Anim.SetFloat("Horizontal", h);
		m_Char.Anim.SetFloat("Vertical", v);
		m_Char.Anim.SetBool("IsMoving", isMoving);
		m_Char.Anim.SetFloat("Idle_Horizontal", m_Char.Direction.x);
		m_Char.Anim.SetFloat("Idle_Vertical", m_Char.Direction.y);
		
		#endregion

		#region Attack

		if (Input.GetButtonDown("Attack"))
		{
			m_Char.Attack();
		}
		
		#endregion

		#region Dash

		if (Input.GetButtonDown("Dash"))
		{
			Vector2 dir = Vector3.Normalize(new Vector2(h, v));
			dir = isMoving ? dir : m_Char.Direction;
			m_Char.Dash(dir);
		}

		#endregion
	}
	
	//Overrides--------------------------------------------------------------------------------------------------------/
	public override void OnHit(Vector2 direction, float damage)
	{
		m_Char.Anim.SetTrigger("Stun");

//		StartCoroutine(LerpHealth(damage, 0.25f));
		m_Char.HealthCurrent -= damage;

		m_Char.PlayHitNoise();
	}

	public override void OnDeath()
	{
		base.OnDeath();
		
		Destroy(GetComponent<Character>());
		Destroy(GetComponent<Rigidbody>());
		Destroy(GetComponent<Collider2D>());
		Destroy(this);
	}

	#endregion

	#region Coroutines---------------------------------------------------------------------------------------------------------/

//	public IEnumerator LerpHealth(float damage, float timer)
//	{
//		float startHealth = m_Char.HealthCurrent;
//		float targetHealth = m_Char.HealthCurrent - damage;
//
//		UI_Gameplay.instance.HealthBar = targetHealth / m_Char.HealthMax;
//		
//		float t = 0;
//		while (t < timer)
//		{
//			t += Time.deltaTime;
//
//			m_Char.HealthCurrent = Mathf.Lerp(startHealth, targetHealth, t / timer);
//			UI_Gameplay.instance.TempHealthBar = m_Char.HealthCurrent / m_Char.HealthMax;
//
//			yield return null;
//		}
//	}
	
	public IEnumerator HapticFeedback(float time, float intensity)
	{
		GamePad.SetVibration(0, 1, 1);
		
		yield return new WaitForSeconds(time);
		
		GamePad.SetVibration(0, 0, 0);
	}

	#endregion
}
