using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericEvent : MonoBehaviour
{
	[SerializeField] private UnityEvent m_GenericEvent;
	public void Invoke()
	{
		m_GenericEvent.Invoke();
	}
}
