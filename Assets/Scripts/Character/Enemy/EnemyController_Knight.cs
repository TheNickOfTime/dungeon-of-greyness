using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_Knight : EnemyController
{
	public override void OnHit(Vector2 direction, float damage)
	{
		bool c1 = m_Char.Direction.x < 0 && direction.x < 0;
		bool c2 = m_Char.Direction.x > 0 && direction.x > 0;

		if(c1 || c2)
		{
			m_Char.Anim.SetTrigger("Stun");
			m_Char.HealthCurrent -= damage;
		}
		else
		{
			m_Char.Anim.SetTrigger("Block");
		}

		State = EnemyState.Idle;
		
		m_Char.PlayHitNoise();
	}
}
