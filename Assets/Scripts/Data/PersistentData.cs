using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
	private static int m_WingsCompleted;
	public static int WingsCompleted
	{
		get { return m_WingsCompleted; }
		set { m_WingsCompleted = value; }
	}
}
