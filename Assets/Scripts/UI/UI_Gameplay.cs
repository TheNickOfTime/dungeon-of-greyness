using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_Gameplay : MonoBehaviour
{
	public static UI_Gameplay instance;

	//Health-----------------------------------------------------------------------------------------------------------/
	[Header("Health")]
	[SerializeField] private Slider m_HealthBar;
	public float HealthBar
	{
		set
		{
			m_HealthBar.value = value;
		}
	}

	[SerializeField] private Slider m_PowerBar;
	public float PowerBar
	{
		set
		{
			m_PowerBar.value = value;
		}
	}

	[SerializeField] private Text m_HealthPackCounter;
	public int HealthPackCounter
	{
		set
		{
			m_HealthPackCounter.text = "x" + value;
			
		}
	}

	//Dialogue---------------------------------------------------------------------------------------------------------/
	[Header("Dialogue")]
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
	[Header("Interaction")]
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
	[Header("Pop-Up")]
	[SerializeField] private GameObject m_PopUpPanel;
	public bool PopUpPanelVisibility
	{
		set
		{
			m_PopUpPanel.SetActive(value);
			PlayerController.instance.Char.CanMove = !value;
			
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

	//Effects----------------------------------------------------------------------------------------------------------/
	[Header("Effects")]
	[SerializeField] private GameObject m_FlashPanel;
	public bool FlashPanel
	{
		set
		{
			m_FlashPanel.SetActive(value);
		}
	}

	//Wave-------------------------------------------------------------------------------------------------------------/
	[Header("Wave")]
	[SerializeField] private GameObject m_WavePanel;
	public GameObject WavePanel
	{
		get
		{
			return m_WavePanel;
		}
	}

	[SerializeField] private Text m_WaveText;
	public string WaveText
	{
		set
		{
			m_WaveText.text = value;
		}
	}

	[SerializeField] private Text m_WaveCountdown;
	public Text WaveCountdown
	{
		get
		{
			return m_WaveCountdown;
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
		HealthPackCounter = PlayerController.instance.HealthPacks;
	}

	private void Update()
	{
		m_InteractionIcon.transform.position = Camera.main.WorldToScreenPoint(PlayerController.instance.transform.position + new Vector3(0, 2.5f));
	}

	#endregion
}
