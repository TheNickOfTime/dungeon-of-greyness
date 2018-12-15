using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HubManager : MonoBehaviour
{
	[SerializeField] private UnityEvent[] m_WingsCompleted;

	private void Start()
	{
		OnHubEnter();
	}

	public void OnHubEnter()
	{
		m_WingsCompleted[PersistentData.instance.WingsCompleted].Invoke();
	}
}
