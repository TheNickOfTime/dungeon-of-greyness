using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
	public static PersistentData instance;

	private static int m_WingsCompleted;
	public int WingsCompleted
	{
		get { return m_WingsCompleted; }
		set { m_WingsCompleted = value; }
	}

	private static int m_HealthPacks;
	public static int HealthPacks
	{
		get { return m_HealthPacks; }
		set
		{
			m_HealthPacks = value;
			Debug.Log(m_HealthPacks);
		}
	}

	private void Awake()
	{
		if(instance != null)
		{
			Destroy(this);
		}
		else
		{
			instance = this;
		}
	}
}
