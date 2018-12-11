using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerInteraction : MonoBehaviour
{
	private enum TriggerEvent
	{
		Enter,
		Exit,
		Stay,
		OnEnable,
		OnDisable
	}
	[SerializeField] private TriggerEvent m_TriggerEvent;
	
	[SerializeField] private UnityEvent m_Event;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (m_TriggerEvent == TriggerEvent.Enter && other.GetComponent<PlayerController>() != null)
		{
			m_Event.Invoke();
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (m_TriggerEvent == TriggerEvent.Exit && other.GetComponent<PlayerController>() != null)
		{
			m_Event.Invoke();
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (m_TriggerEvent == TriggerEvent.Stay && other.GetComponent<PlayerController>() != null)
		{
			m_Event.Invoke();
		}
	}

	private void OnEnable()
	{
		if (m_TriggerEvent == TriggerEvent.OnEnable)
		{
			m_Event.Invoke();
		}
	}

	private void OnDisable()
	{
		if (m_TriggerEvent == TriggerEvent.OnDisable)
		{
			m_Event.Invoke();
		}
	}
}
