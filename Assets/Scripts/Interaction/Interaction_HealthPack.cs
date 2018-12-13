using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_HealthPack : Interaction
{
	public override void TriggerInteraction()
	{
		PlayerController.instance.HealthPacks++;
		Destroy(gameObject);
	}
}
