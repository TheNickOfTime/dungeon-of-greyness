using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyController_Boss : EnemyController
{
	#region Values & Properties------------------------------------------------------------------------------------------------/

	[Header("Boss Controller")]

	//Spawn Objects----------------------------------------------------------------------------------------------------/
	[SerializeField] private GameObject m_Hazard;
	[SerializeField] private GameObject m_SpawnIndicator;
	
	//Hazard-----------------------------------------------------------------------------------------------------------/
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
					m_Char.CanMove = false;
					m_Char.Rig.velocity = Vector2.zero;
					break;
				case EnemyState.Alert:
					m_Char.Anim.SetBool("IsMoving", true);
					m_Char.CanMove = true;
//					m_CheeseCounter = 0;
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
	
	private GameObject[] m_NextSpawn;
	private GameObject[] m_SpawnIndicators;
	
	//UI---------------------------------------------------------------------------------------------------------------/
	private Slider m_HealthBar;
	
	//Misc-------------------------------------------------------------------------------------------------------------/
	private float m_DefaultAttackRadius = 2;
	private float m_RangedAttackRadius = 10;
	private float m_SpawnAttackRadius = 5;

	private int m_CheeseCounter = 0;

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
				m_MinimumDistance = 2;
				break;
			case AttackType.Ranged:
				m_AttackRadius = m_RangedAttackRadius;
				m_MinimumDistance = 6;
				break;
			case AttackType.Spawn:
				m_AttackRadius = m_SpawnAttackRadius;
				m_MinimumDistance = 4;
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

	public void ResetCheeseCounter()
	{
		m_CheeseCounter = 0;
	}

	#endregion
	
	#region Ranged-------------------------------------------------------------------------------------------------------------/

	private void RandomPosition()
	{
		Vector2 playerPos = PlayerController.instance.transform.position;
		Vector2 newPos = Vector2.zero;

		while (newPos == Vector2.zero)
		{
			Vector2 tempPos = Random.insideUnitCircle * m_AlertRadius + playerPos;
			if (!CheckForWall(tempPos) || tempPos == Vector2.zero)
			{
				newPos = tempPos;
			}
		}

		m_Char.Rig.MovePosition(newPos);
	}
	
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

	public void ChooseEnemies()
	{
		int randomGroup = Random.Range(0, m_SpawnGroups.Length);
		SpawnGroup group = m_SpawnGroups[randomGroup];
		m_NextSpawn = new GameObject[group.group.Length];
		m_SpawnIndicators = new GameObject[group.group.Length];

		for (int i = 0; i < group.group.Length; i++)
		{
			var spawn = group.group[i];
			Vector2 pos = spawn.relativeCoordinates + (Vector2)PlayerController.instance.transform.position;
			if (!CheckForWall(pos))
			{
				GameObject enemy = Instantiate(spawn.enemy, pos, Quaternion.identity, transform.parent);
				enemy.SetActive(false);
				EnemyController enemyToAdd = enemy.GetComponent<EnemyController>();
				EnemyManager.instance.CurrentWave.Add(enemyToAdd);
				m_NextSpawn[i] = enemy;

				GameObject warnings = Instantiate(m_SpawnIndicator, enemy.transform.position, Quaternion.identity);
				m_SpawnIndicators[i] = warnings;
			}
		}
	}

	private void SpawnEnemyGroup()
	{
		if (m_NextSpawn != null)
		{
			for (int i = 0; i < m_NextSpawn.Length; i++)
			{
				Destroy(m_SpawnIndicators[i]);
				if (m_NextSpawn[i] != null)
				{
					m_NextSpawn[i].SetActive(true);
				}
			}
		}
	}

	public void DestoryIndicators()
	{
		if (m_SpawnIndicators != null)
		{
			foreach (GameObject spawnIndicator in m_SpawnIndicators)
			{
				Destroy(spawnIndicator);
			}
		}
	}

	#endregion

	public override void OnHit(Vector2 direction, float damage)
	{
//		bool isMeleeAttack = m_Char.Anim.GetCurrentAnimatorStateInfo(0).IsName("Melee Attack");
		bool isSpawnAttack = m_Char.Anim.GetCurrentAnimatorStateInfo(0).IsName("Spawn Attack");
		bool isRangeAttack = m_Char.Anim.GetCurrentAnimatorStateInfo(0).IsName("Range Attack");

		if (isSpawnAttack || isRangeAttack)
		{
			m_Char.PlayNoise("Block");
			m_Char.HealthCurrent -= damage * 0.5f;
			m_HealthBar.value = Char.HealthCurrent / Char.HealthMax;
			Instantiate(m_Hazard, transform.position, Quaternion.identity);
			return;
		}
		
		//Adds cheese to avoid hit spamming
		m_CheeseCounter++;
		
		if (m_CheeseCounter < 6)//If not cheesing
		{
			//Checks conditions for hit or block
			bool c1 = m_Char.Direction.x < 0 && direction.x < 0;
			bool c2 = m_Char.Direction.x > 0 && direction.x > 0;
			bool stunThreshold = damage > 3;
		
			if(c1 || c2 || stunThreshold)
			{
				m_Char.Anim.SetTrigger("Stun");
				Instantiate(m_Particles, transform.position, Quaternion.identity);
				m_Char.HealthCurrent -= damage;
			}
			else
			{
				m_Char.Anim.SetTrigger("Block");
			}
			
			NextAttack();
		}
		else
		{
			m_Char.Anim.Play("Teleport");
			Instantiate(m_Hazard, transform.position, Quaternion.identity);
			m_CheeseCounter = 0;
		}
		
		m_HealthBar.value = Char.HealthCurrent / Char.HealthMax;

		State = EnemyState.Alert;
	}
}
