using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : Controller
{
	//Singleton--------------------------------------------------------------------------------------------------------/
	public static PlayerController instance;
	
	#region Values-------------------------------------------------------------------------------------------------------------/

//	private float m_TempHealth;

	[SerializeField] private float m_InputTime;
	
	#endregion
	
	#region Properties---------------------------------------------------------------------------------------------------------/
	
	public float TimeScale
	{
		set { Time.timeScale = value; }
	}
	
	#endregion

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
//		m_Char.SpawnSpriteTrail();
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
			if (m_Char.m_CanMove)
			{
				m_Char.Direction = dir;
			}
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

		#region Heavy Attack
		
		
		if (Input.GetButton("Attack"))
		{
			bool animCheck =  m_Char.Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_01") &&
			                  m_InputTime >= m_Char.Anim.GetCurrentAnimatorStateInfo(0).length - 0.05f;
			
			m_InputTime += Time.deltaTime;
			if (animCheck)
			{
				m_Char.HeavyCharge();
			}
		}

		if (Input.GetButtonUp("Attack"))
		{
			m_InputTime = 0;

			if (m_Char.Anim.GetCurrentAnimatorStateInfo(0).IsName("Heavy Charge"))
			{
				m_Char.HeavyAttack();
			}
		}

		#endregion

		#region Dash

		if (Input.GetButtonDown("Dash") && m_Char.m_CanMove)
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
		if(damage > 1)
		{
			m_Char.Anim.SetTrigger("Stun Heavy");
		}
		else
		{
			m_Char.Anim.SetTrigger("Stun Light");
		}

//		StartCoroutine(LerpHealth(damage, 0.25f));
		m_Char.HealthCurrent -= damage;

		m_Char.PlayHitNoise();
		Shaker.instance.HapticShake(0.35f, 0.75f);
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

	public IEnumerator LerpHealth(float damage, float timer)
	{
		float startHealth = m_Char.HealthCurrent;
		float targetHealth = m_Char.HealthCurrent - damage;

		UI_Gameplay.instance.HealthBar = targetHealth / m_Char.HealthMax;

		float t = 0;
		while (t < timer)
		{
			t += Time.deltaTime;

			m_Char.HealthCurrent = Mathf.Lerp(startHealth, targetHealth, t / timer);
			UI_Gameplay.instance.TempHealthBar = m_Char.HealthCurrent / m_Char.HealthMax;

			yield return null;
		}
	}

	public IEnumerator HapticFeedback(float time, float intensity)
	{
		GamePad.SetVibration(0, 1, 1);
		
		yield return new WaitForSeconds(time);
		
		GamePad.SetVibration(0, 0, 0);
	}

	#endregion
}