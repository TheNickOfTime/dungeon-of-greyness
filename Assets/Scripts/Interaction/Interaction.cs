using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		PlayerController player = other.GetComponent<PlayerController>();
		if (player != null)
		{
			player.CurrentInteractionObject = this;
		}
		EnterTrigger();
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		PlayerController player = other.GetComponent<PlayerController>();
		if (player != null)
		{
			player.CurrentInteractionObject = null;
		}
		ExitTrigger();
	}

	public abstract void TriggerInteraction();

	protected virtual void EnterTrigger()
	{
		
	}
	
	protected virtual void ExitTrigger()
	{
		
	}
}
