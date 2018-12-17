using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour, IHittable
{
	#region Components---------------------------------------------------------------------------------------------------------/

	protected Character m_Char;
	protected SpriteRenderer m_Ren;
	protected Rigidbody2D m_Rig;

	[Header("Controller")]
	[SerializeField] protected GameObject m_Particles;

	protected Interaction m_CurrentInteraction;

	#endregion

	#region Properties---------------------------------------------------------------------------------------------------------/

	public Character Char
	{
		get { return m_Char; }
	}

	public Rigidbody2D Rig
	{
		get { return m_Rig; }
	}

	public Interaction CurrentInteractionObject
	{
		get
		{
			return m_CurrentInteraction;
		}
		set
		{
			m_CurrentInteraction = value;
			UI_Gameplay.instance.InteractionIconVisibility = value != null;
		}
	}

	#endregion

	protected virtual void Awake()
	{
		m_Char = GetComponent<Character>();
		m_Ren = GetComponent<SpriteRenderer>();
		m_Rig = GetComponent<Rigidbody2D>();
	}

	//Reactions--------------------------------------------------------------------------------------------------------/
	public virtual void OnHit(Vector2 direction, float damage)
	{
//		Debug.Log(m_Char.Anim.GetCurrentAnimatorStateInfo(0).IsName("Stun"));
		if (damage > 1)
		{
			m_Char.Anim.SetTrigger("Stun");
		}
		m_Char.HealthCurrent -= damage;
		Instantiate(m_Particles, transform.position, Quaternion.identity);
	}

	public virtual void OnDeath()
	{
		m_Char.Anim.SetTrigger("Stun");
		m_Char.Anim.SetTrigger("Die");
	}
}
