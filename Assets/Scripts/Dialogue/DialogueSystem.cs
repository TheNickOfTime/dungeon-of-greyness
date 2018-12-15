using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueSystem : MonoBehaviour
{
	[SerializeField] private DialogueData m_DialogueData;
	public DialogueData DialogueData
	{
		set
		{

			m_DialogueData = value;
			m_LineIndex = 0;
			m_HasFinished = false;
		}
	}

	[SerializeField] private GenericEvent m_FinishEvent;
	public GenericEvent FinishEvent
	{
		set
		{
			//Debug.Log(m_FinishEvent);
			m_FinishEvent = value;
		}
	}
	
	public enum PostFinishBehavior
	{
		RepeatLastLine,
		Repeat,
		Disable
	}
	[SerializeField] private PostFinishBehavior m_OnFinish;
	public PostFinishBehavior OnFinish
	{
		set
		{
			m_OnFinish = value;
		}
	}

	private int m_LineIndex = 0;
	private bool m_HasFinished = false;

	public void PlayLine()
	{
		if (m_LineIndex >= m_DialogueData.m_Lines.Length)
		{
			PlayerController.instance.Char.CanMove = true;
			UI_Gameplay.instance.DialoguePanelVisibility = false;
//			UI_Gameplay.instance.InteractionIconVisibility = true;
			
			switch (m_OnFinish)
			{
				case PostFinishBehavior.RepeatLastLine:
					m_LineIndex = m_DialogueData.m_Lines.Length - 1;
					break;
				
				case PostFinishBehavior.Repeat:
					m_LineIndex = 0;
					break;
				
				case PostFinishBehavior.Disable:
					DisableSystem();
					break;
			}

			if (!m_HasFinished)
			{
				m_HasFinished = true;
				m_FinishEvent.Invoke();
			}
			
			return;
		}

		PlayerController.instance.Char.CanMove = false;
		
		UI_Gameplay.instance.DialoguePanelVisibility = true;
//		UI_Gameplay.instance.InteractionIconVisibility = false;
		
		StartCoroutine(TypeText());
	}

	private void DisableSystem()
	{
		Interaction_Dialogue interaction = GetComponent<Interaction_Dialogue>();
		if (interaction != null)
		{
			interaction.enabled = false;
		}

		enabled = false;
	}

	private IEnumerator TypeText()
	{
		string currentLine = m_DialogueData.m_Lines[m_LineIndex];
		string currentString = "";
		
		for (int i = 0; i < currentLine.Length; i++)
		{
			currentString += currentLine[i];
			UI_Gameplay.instance.DialogueText = currentString;
			yield return null;
		}

		m_LineIndex++;
	}
}
