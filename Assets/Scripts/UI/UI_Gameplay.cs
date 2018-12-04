using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Gameplay : MonoBehaviour
{
	public static UI_Gameplay instance;
	
	[SerializeField] private Slider m_HealthBar;
	public float HealthBar
	{
		set
		{
			m_HealthBar.value = value;
		}
	}

	[SerializeField] private Slider m_TempHealthBar;
	public float TempHealthBar
	{
		set
		{
			m_TempHealthBar.value = value;
		}
	}

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(this);
		}
		else
		{
			instance = this;
		}
	}
}
