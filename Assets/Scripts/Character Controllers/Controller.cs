using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour, IHittable
{
	#region Components---------------------------------------------------------------------------------------------------------/

	protected Character m_Char;
	protected SpriteRenderer m_Ren;

	[Header("Controller")]
	[SerializeField] protected GameObject m_Particles;

	protected Interaction m_CurrentInteraction;

	#endregion

	#region Properties---------------------------------------------------------------------------------------------------------/

	public Character Char
	{
		get { return m_Char; }
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
	}

	//Reactions--------------------------------------------------------------------------------------------------------/
	public virtual void OnHit(Vector2 direction, float damage)
	{
		m_Char.Anim.SetTrigger("Stun");
		m_Char.HealthCurrent -= damage;
		m_Char.PlayHitNoise();
	}

	public virtual void OnDeath()
	{
		m_Char.Anim.SetTrigger("Die");
		m_Char.PlayDeathNoise();
	}
}
