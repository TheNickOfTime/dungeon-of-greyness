using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class EnemyController_Wizard : EnemyController
{
	[Header("Wizard")]
	[SerializeField] private GameObject m_Hazard;
	
	[System.Serializable]
	private struct HazardPositions
	{
		public Vector2[] positions;
	}

	[SerializeField] private HazardPositions[] m_HazardPositions;

	private Vector2 m_NextSpawnPos;

	
	protected override void AlertState()
	{
		
	}

	protected override void AttackState()
	{
		
	}

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
		return Physics2D.OverlapCircle(pos, GetComponent<CircleCollider2D>().radius, ~1 << LayerMask.NameToLayer("Water")) != null;
	}
	
	protected override bool PlayerInSight()
	{
		bool didHit = Physics2D.Linecast(transform.position, m_Target.position, 1 << LayerMask.NameToLayer("Environment"));
		return !didHit;
	}
	
	public void RandomAttackPattern()
	{
		StartCoroutine(SpawnHazards());
	}

	public void SetNextSpawnPosition()
	{
		m_NextSpawnPos = PlayerController.instance.transform.position;
	}

	private IEnumerator SpawnHazards()
	{
		int seed = Random.Range(0, m_HazardPositions.Length);
		Vector2 playerPos = m_NextSpawnPos;
		for (int i = 0; i < m_HazardPositions[seed].positions.Length; i++)
		{
			Vector2 pos = m_HazardPositions[seed].positions[i] * 2 + playerPos;
			Instantiate(m_Hazard, pos, Quaternion.identity);
			yield return null;
		}
	}
}
