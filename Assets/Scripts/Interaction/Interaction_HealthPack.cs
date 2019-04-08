using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_HealthPack : Interaction
{
	private Collider2D m_Col;
	
	private void Awake()
	{
		m_Col = GetComponent<Collider2D>();
	}

	public override void TriggerInteraction()
	{
		PlayerController.instance.HealthPacks++;
		PersistentData.HealthPacks = PlayerController.instance.HealthPacks;
		Destroy(gameObject);
	}
}
