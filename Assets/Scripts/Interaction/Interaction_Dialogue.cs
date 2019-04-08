using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_Dialogue : Interaction
{
	private DialogueSystem m_DialogueSystem;

	private void Awake()
	{
		m_DialogueSystem = GetComponent<DialogueSystem>();
	}

	public override void TriggerInteraction()
	{
		m_DialogueSystem.PlayLine();
	}

	protected override void ExitTrigger()
	{
		m_DialogueSystem.EndDialogue();
	}
}
