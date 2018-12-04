using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_KillAllEnemies : Objective
{
	[SerializeField] private EnemyController[] m_Enemies;
	
	protected override bool Condition()
	{
		for (int i = 0; i < m_Enemies.Length; i++)
		{
			if (m_Enemies[i] != null)
			{
				return false;
			}
		}

		return true;
	}

//	protected override void Evaluate()
//	{
////		base.Evaluate();
////		Debug.Log(Condition());
//	}
}
