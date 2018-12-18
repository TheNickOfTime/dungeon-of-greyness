
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

	private float m_Power;
	
	private float m_AttackInputTime;
	private float m_DashInputTime;
	private float m_HealInputTime;

	private static bool m_CanDash;
	private static bool m_CanSuperDash;
	private static bool m_CanHeavyHit;

	private bool m_CanDashAgain = true;
	private bool m_CanHealAgain = true;

	[SerializeField] private int m_HealthPacks = 0;

	private int m_RoomsCompleted;
	
	#endregion
	
	#region Properties---------------------------------------------------------------------------------------------------------/
	
	public float TimeScale
	{
		set { Time.timeScale = value; }
	}

	public float Power
	{
		get { return m_Power; }
		set
		{
			m_Power = Mathf.Clamp(value, 0, 1);
			UI_Gameplay.instance.PowerBar = value;
		}
	}

	public bool CanDash
	{
		get { return m_CanDash; }
		set { m_CanDash = value; }
	}

	public bool CanSuperDash
	{
		get { return m_CanSuperDash; }
		set { m_CanSuperDash = value; }
	}

	public bool CanHeavyHit
	{
		get { return m_CanHeavyHit; }
		set { m_CanHeavyHit = value; }
	}

	public int HealthPacks
	{
		get { return m_HealthPacks; }
		set
		{
			m_HealthPacks = value;
			UI_Gameplay.instance.HealthPackCounter = value;
		}
	}

	public int RoomsCompleted
	{
		get
		{
			return m_RoomsCompleted;
		}
		set
		{
			m_RoomsCompleted = value;
		}
	}

	#endregion

	#region Monobehaviour------------------------------------------------------------------------------------------------------/

	protected override void Awake()
	{
		if (instance != null)
		{
			gameObject.SetActive(false);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	    
		base.Awake();

		GameObject spawn = new GameObject("Player Spawn");
		spawn.transform.position = transform.position;
	}

	private void Start()
	{
		HealthPacks++;
	}

	private void Update()
	{	
		PlayerInput();

		Power += Time.deltaTime * 0.25f;
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
		if (isMoving && m_Char.CanMove)
		{
			Vector2 dir = Mathf.Abs(h) > Mathf.Abs(v) ? new Vector2(h, 0) : new Vector2(0, v);
			dir = Vector3.Normalize(dir);
			if (m_Char.CanMove)
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

		if (Input.GetButtonDown("Attack") && Power > 0.1f)
		{
			m_Char.Attack();
			Power -= 0.15f;
		}
		
		#endregion

		#region Heavy Attack


		if (CanHeavyHit)
		{
			if (Input.GetButton("Attack") && Power > 0.5f)
			{
				bool animCheck =  m_Char.Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_01") &&
				                  m_AttackInputTime >= m_Char.Anim.GetCurrentAnimatorStateInfo(0).length - 0.05f;
			
				m_AttackInputTime += Time.deltaTime;
				if (animCheck)
				{
					m_Char.HeavyCharge();
					Power -= 1.0f;
					m_AttackInputTime = 0;
				}
			}
			if (Input.GetButtonUp("Attack"))
			{
				m_AttackInputTime = 0;

//				if (m_Char.Anim.GetCurrentAnimatorStateInfo(0).IsName("Heavy Charge"))
//				{
//					m_Char.HeavyAttack();
//				}
			}
		}

		#endregion

		#region Dash

		if (CanDash)
		{
			if (Input.GetButtonDown("Dash"))
			{
				m_Char.Anim.SetBool("Dash", true);
			}

			if (Input.GetButton("Dash") && CanSuperDash)
			{
				m_DashInputTime += Time.deltaTime;
				if (m_CanDashAgain && m_DashInputTime > 0.25f && Power > 0.35f)
				{
					m_Char.Anim.SetTrigger("Dash Heavy");
					m_Char.Anim.SetBool("Dash", false);
					
					Vector2 dir = Vector3.Normalize(new Vector2(h, v));
					dir = isMoving ? dir : m_Char.Direction;
					m_Char.Dash(dir);
					Power -= 0.5f;
					
					m_CanDashAgain = false;
					m_DashInputTime = 0;
				}
			}

			if (Input.GetButtonUp("Dash"))
			{
				if (m_DashInputTime < 0.25f && Power > 0.15f)
				{
					m_Char.Anim.SetTrigger("Dash Regular");
					m_Char.Anim.SetBool("Dash", false);
					
					Vector2 dir = Vector3.Normalize(new Vector2(h, v));
					dir = isMoving ? dir : m_Char.Direction;
					m_Char.Dash(dir);
					Power -= 0.25f;
				}
				m_Char.Anim.SetBool("Dash", false);
				m_CanDashAgain = true;
				m_DashInputTime = 0;
			}
			
//			if (Input.GetButtonUp("Dash") && m_Char.CanMove && Power > 0.15f)
//			{
//				Vector2 dir = Vector3.Normalize(new Vector2(h, v));
//				dir = isMoving ? dir : m_Char.Direction;
//				m_Char.Dash(dir);
//				Power -= 0.25f;
//			}
		}

		#endregion

		#region Heal

//		if (Input.GetButtonDown("Heal") && m_HealthPacks > 0)
//		{
//			HealthPacks -= 1;
//			m_Char.HealthCurrent = m_Char.HealthMax;
//		}

		if (Input.GetButton("Heal") && m_HealthPacks > 0)
		{
			m_HealInputTime += Time.deltaTime;
			if (m_HealInputTime > 0.25 && m_CanHealAgain)
			{
				m_CanHealAgain = false;
				HealthPacks -= 1;
				m_Char.HealthCurrent = m_Char.HealthMax;
				m_Char.PlayNoise("Heal");
				m_HealInputTime = 0;
			}
		}

		if (Input.GetButtonUp("Heal"))
		{
			m_CanHealAgain = true;
			m_HealInputTime = 0;
		}

		#endregion

		#region Interaction

		if (Input.GetButtonDown("Interact"))
		{
			if (CurrentInteractionObject != null)
			{
				CurrentInteractionObject.TriggerInteraction();
			}
		}

		#endregion

		#region PlayerCheats

#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.F1))
		{
			CanDash = true;
			CanSuperDash = true;
			CanHeavyHit = true;
		}
#endif

		#endregion
	}
	
	//Overrides--------------------------------------------------------------------------------------------------------/
	public override void OnHit(Vector2 direction, float damage)
	{
		if(!m_Char.CanRecieveDamage) return;
		
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

		Shaker.instance.HapticShake(0.35f, 0.75f);
	}

	public override void OnDeath()
	{	
		foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
		{
			enemy.enabled = false;
			enemy.Rig.velocity = Vector2.zero;
//			enemy.GetComponent<Animator>().enabled = false;
		}
		
		base.OnDeath();

		StartCoroutine(RespawnSequence());


//		Destroy(GetComponent<Character>());
//		Destroy(GetComponent<Rigidbody>());
//		Destroy(GetComponent<Collider2D>());
//		Destroy(this);
	}

	#endregion

	#region Coroutines---------------------------------------------------------------------------------------------------------/

	public IEnumerator HapticFeedback(float time, float intensity)
	{
		GamePad.SetVibration(0, 1, 1);
		
		yield return new WaitForSeconds(time);
		
		GamePad.SetVibration(0, 0, 0);
	}

	public IEnumerator RespawnSequence()
	{
		yield return new WaitForSecondsRealtime(1.5f);
		SceneLoader.instance.LoadLevel("", "");
		yield return new WaitForSecondsRealtime(0.5f);
		m_Char.HealthCurrent = m_Char.HealthMax;
		m_Char.Anim.Play("Idle");
	}

	#endregion
}