using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyController_Boss : EnemyController_Knight
{
	#region Values & Properties------------------------------------------------------------------------------------------------/

	[Header("Boss Controller")]
	
	//Hazard-----------------------------------------------------------------------------------------------------------/
	[SerializeField] private GameObject m_Hazard;
	[System.Serializable] private struct HazardPositions
	{
		public Vector2[] positions;
	}
	[SerializeField] private HazardPositions[] m_HazardPositions;

	//Attacks & States-------------------------------------------------------------------------------------------------/
	protected override EnemyState State
	{
		get
		{
			return m_State;
		}
		set
		{
			m_State = value;
			
			switch (value)
			{
				case EnemyState.Idle:
					m_Char.Anim.SetBool("IsMoving", false);
					m_Char.m_CanMove = false;
					m_Char.Rig.velocity = Vector2.zero;
					break;
				case EnemyState.Alert:
					m_Char.Anim.SetBool("IsMoving", true);
					m_Char.m_CanMove = true;
					break;
				case EnemyState.Attack:
					switch (m_CurrentAttack)
					{
						case AttackType.Melee:
							m_Char.Anim.SetTrigger("Attack");
							break;
						case AttackType.Ranged:
							m_Char.Anim.SetTrigger("Range");
							break;
						case AttackType.Spawn:
							m_Char.Anim.SetTrigger("Spawn");
							break;
					}
					break;
			}
		}
	}

	private enum AttackType
	{
		Melee,
		Ranged,
		Spawn
	}
	[System.Serializable] private struct AttackPattern
	{
		public AttackType[] attackPattern;
	}
	[SerializeField] private AttackPattern[] m_AttackPatterns;

	private AttackType m_CurrentAttack;
	
	private float[] m_Stages = new float[]{-0.25f, -0.65f};

	private int m_StageIndex = 0;
	private int m_AttackIndex = 0;

	//Spawn Groups-----------------------------------------------------------------------------------------------------/
	[System.Serializable] private struct SpawnGroup
	{
		[System.Serializable] public struct Group
		{
			public GameObject enemy;
			public Vector2 relativeCoordinates;
		}
		public Group[] group;
	}
	[SerializeField] private SpawnGroup[] m_SpawnGroups;
	
	//UI---------------------------------------------------------------------------------------------------------------/
	private Slider m_HealthBar;
	
	//Misc-------------------------------------------------------------------------------------------------------------/
	private float m_DefaultAttackRadius = 2;
	private float m_RangedAttackRadius = 5;
	private float m_SpawnAttackRadius = 4;

	#endregion
	
	#region Monobehavior-------------------------------------------------------------------------------------------------------/

	protected override void Start()
	{
		base.Start();

		m_HealthBar = transform.GetChild(0).GetChild(0).GetComponent<Slider>();
	}

	protected override void Update()
	{
		CheckStage();

		m_HealthBar.value = Char.HealthCurrent / Char.HealthMax;
		
		base.Update();
	}

	#endregion

	#region State--------------------------------------------------------------------------------------------------------------/

	private void CheckStage()
	{
		if (m_StageIndex >= m_Stages.Length) return;
		
//		Debug.Log(m_Stages[m_StageIndex]);
		
		if (Char.HealthCurrent / Char.HealthMax <=  1 + m_Stages[m_StageIndex])
		{
			m_StageIndex++;
		}
	}
	
	public void NextAttack()
	{
		m_AttackIndex++;
		if (m_AttackIndex >= m_AttackPatterns[m_StageIndex].attackPattern.Length)
		{
			m_AttackIndex = 0;
		}

		m_CurrentAttack = m_AttackPatterns[m_StageIndex].attackPattern[m_AttackIndex];

		switch (m_CurrentAttack)
		{
			case AttackType.Melee:
				m_AttackRadius = m_DefaultAttackRadius;
				break;
			case AttackType.Ranged:
				m_AttackRadius = m_RangedAttackRadius;
				break;
			case AttackType.Spawn:
				m_AttackRadius = m_SpawnAttackRadius;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		State = EnemyState.Alert;
	}

	#endregion

	#region Attack-------------------------------------------------------------------------------------------------------------/

	protected override void AttackState()
	{
		m_Char.Move(Vector2.zero);
		m_AttackCooldownCurrent = m_AttackCooldownMax;
	}

	#endregion
	
	#region Ranged-------------------------------------------------------------------------------------------------------------/

	private bool CheckForWall(Vector2 pos)
	{
		return Physics2D.OverlapCircle(pos, 0.5f) != null;
	}
	
	public void RandomHazardPattern()
	{
		StartCoroutine(SpawnHazards());
	}

	private IEnumerator SpawnHazards()
	{
		int seed = Random.Range(0, m_HazardPositions.Length);
		Vector2 playerPos = (Vector2) PlayerController.instance.transform.position;
		for (int i = 0; i < m_HazardPositions[seed].positions.Length; i++)
		{
			Vector2 pos = m_HazardPositions[seed].positions[i] * 2 + playerPos;
			Instantiate(m_Hazard, pos, Quaternion.identity);
			yield return null;
		}
	}

	#endregion

	#region Spawn--------------------------------------------------------------------------------------------------------------/

	private void SpawnEnemyGroup()
	{
		int randomGroup = Random.Range(0, m_SpawnGroups.Length);
		SpawnGroup group = m_SpawnGroups[randomGroup];
		foreach (var spawn  in group.group)
		{
			Vector2 pos = spawn.relativeCoordinates + (Vector2)PlayerController.instance.transform.position;
			if (!CheckForWall(pos))
			{
				GameObject obj = Instantiate(spawn.enemy, pos, Quaternion.identity, transform.parent);
				EnemyController enemyToAdd = obj.GetComponent<EnemyController>();
				EnemyManager.instance.CurrentWave.Add(enemyToAdd);
			}
		}
	}

	#endregion

	public override void OnDeath()
	{
		base.OnDeath();
	}

	#region Utility------------------------------------------------------------------------------------------------------------/

	public void PlaySpawnNoise()
	{
		SFXManager.PlayClipAtPoint(m_Char.m_AttackSounds[0], transform.position);
	}

	public void PlayHazardNoise()
	{
		SFXManager.PlayClipAtPoint(m_Char.m_AttackSounds[1], transform.position);
	}

	#endregion
}
