using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
	private PlayerController player;
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		player = other.GetComponent<PlayerController>();
		if (player != null)
		{
			EnterTrigger();
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (player != null)
		{
			player.CurrentInteractionObject = this;
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (player != null)
		{
			player.CurrentInteractionObject = null;
			player = null;
			ExitTrigger();
		}
	}

	public abstract void TriggerInteraction();

	protected virtual void EnterTrigger()
	{
		
	}
	
	protected virtual void ExitTrigger()
	{
		
	}
}
