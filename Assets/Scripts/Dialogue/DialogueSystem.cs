using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
	
	[SerializeField] private AudioClip m_TypingNoise;
	
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

	private Coroutine m_TypingSequence;
	private bool m_IsTyping = false;

	public void PlayLine()
	{
		if (m_LineIndex >= m_DialogueData.m_Lines.Length)
		{
			//PlayerController.instance.Char.Anim.enabled = true;
			//PlayerController.instance.Char.Anim.Play("Idle");
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
				if (m_FinishEvent != null)
				{
					m_FinishEvent.Invoke();
				}
			}
			
			return;
		}

		//PlayerController.instance.Char.Anim.enabled = false;
		//PlayerController.instance.Char.CanMove = false;
		
		UI_Gameplay.instance.DialoguePanelVisibility = true;
//		UI_Gameplay.instance.InteractionIconVisibility = false;

		if (m_TypingSequence != null)
		{
			StopCoroutine(m_TypingSequence);
		}
		m_TypingSequence = StartCoroutine(TypeText());
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
		
		if (m_IsTyping)
		{
			UI_Gameplay.instance.DialogueText = currentLine;
			m_IsTyping = false;
			m_LineIndex++;
			yield break;
		}

		m_IsTyping = true;
		for (int i = 0; i < currentLine.Length; i++)
		{
			currentString += currentLine[i];
			UI_Gameplay.instance.DialogueText = currentString;

			if (currentLine[i] == ' ' || currentLine[i] == ',')
			{
				yield return new WaitForSeconds(0.05f);
			}
			else
			{
				SFXManager.PlayClipAtPoint(m_TypingNoise, transform.position).volume = 0.5f;
			}
			yield return null;
		}
		m_IsTyping = false;
		
		m_LineIndex++;
	}
}
