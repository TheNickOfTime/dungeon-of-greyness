using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
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

	//Dialogue---------------------------------------------------------------------------------------------------------/
	[SerializeField] private GameObject m_DialoguePanel;
	public bool DialoguePanelVisibility
	{
		set
		{
			m_DialoguePanel.SetActive(value);
		}
	}
	
	[SerializeField] private Text m_DialogueText;
	public string DialogueText
	{
		set
		{
			m_DialogueText.text = value;
		}
	}
	
	//Interaction------------------------------------------------------------------------------------------------------/
	[SerializeField] private GameObject m_InteractionIcon;
	public bool InteractionIconVisibility
	{
		get
		{
			return m_InteractionIcon.activeSelf;
		}
		set
		{
			m_InteractionIcon.SetActive(value);
		}
	}
	
	//Pop Up-----------------------------------------------------------------------------------------------------------/
	[SerializeField] private GameObject m_PopUpPanel;
	public bool PopUpPanelVisibility
	{
		set
		{
			m_PopUpPanel.SetActive(value);
			PlayerController.instance.Char.m_CanMove = !value;
			
		}
	}
	
	[SerializeField] private Image m_PopUpIcon;
	public Sprite PopUpIcon
	{
		set
		{
			m_PopUpIcon.sprite = value;
		}
	}
	
	[SerializeField] private Text m_PopUpText;
	public string PopUpText
	{
		set
		{
			m_PopUpText.text = value;
		}
	}


	#region Monobehavior-------------------------------------------------------------------------------------------------------/

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
	}

	private void Start()
	{
		HealthBar = PlayerController.instance.Char.HealthCurrent / PlayerController.instance.Char.HealthMax;
	}

	private void Update()
	{
		m_InteractionIcon.transform.position = Camera.main.WorldToScreenPoint(PlayerController.instance.transform.position + new Vector3(0, 2.5f));
	}

	#endregion
}
