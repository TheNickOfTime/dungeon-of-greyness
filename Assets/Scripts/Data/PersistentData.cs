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
