using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Gameplay : UI
{
	public static UI_Gameplay instance;

	public enum Panels
	{
		Gameplay,
		Pause
	}
	[SerializeField] private Dictionary<Panels, GameObject> m_Panels;
	private GameObject m_CurrentPanel;
	private GameObject m_PreviousPanel;

	#region Gameplay Panel
	
	//Health-----------------------------------------------------------------------------------------------------------/
	
	[Header("Health")]
	[FoldoutGroup("Gameplay Panel")]
	[SerializeField] private Slider m_HealthBar;
	public float HealthBar
	{
		set
		{
			m_HealthBar.value = value;
		}
	}

	[FoldoutGroup("Gameplay Panel")]
	[SerializeField] private Slider m_PowerBar;
	public float PowerBar
	{
		set
		{
			m_PowerBar.value = value;
		}
	}

	[FoldoutGroup("Gameplay Panel")]
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
	[FoldoutGroup("Gameplay Panel")]
	[SerializeField] private GameObject m_DialoguePanel;
	public bool DialoguePanelVisibility
	{
		set
		{
			m_DialoguePanel.SetActive(value);
		}
	}
	
	[FoldoutGroup("Gameplay Panel")]
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
	[FoldoutGroup("Gameplay Panel")]
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
	[FoldoutGroup("Gameplay Panel")]
	[SerializeField] private GameObject m_PopUpPanel;
	public bool PopUpPanelVisibility
	{
		set
		{
			m_PopUpPanel.SetActive(value);
			PlayerController.instance.Char.CanMove = !value;
			
		}
	}
	
	[FoldoutGroup("Gameplay Panel")]
	[SerializeField] private Image m_PopUpIcon;
	public Sprite PopUpIcon
	{
		set
		{
			m_PopUpIcon.sprite = value;
		}
	}
	
	[FoldoutGroup("Gameplay Panel")]
	[SerializeField] private Text m_PopUpText;
	public string PopUpText
	{
		set
		{
			m_PopUpText.text = value;
		}
	}

	//Upgrade----------------------------------------------------------------------------------------------------------/
	[Header("Upgrade")]
	[FoldoutGroup("Gameplay Panel")]
	[SerializeField] private GameObject m_UpgradePanel;
	[FoldoutGroup("Gameplay Panel")]
	[SerializeField] private Button m_HeavyHitButton;
	[FoldoutGroup("Gameplay Panel")]
	[SerializeField] private Button m_SuperDashButton;
	public bool UpgradePanelVisibility
	{
		set
		{
			m_UpgradePanel.SetActive(value);
			PlayerController.instance.Char.CanMove = !value;
			EventSystem.current.SetSelectedGameObject(m_HeavyHitButton.gameObject);
			
			
			if(PlayerController.instance.CanHeavyHit)
			{
				m_HeavyHitButton.interactable = false;
				EventSystem.current.SetSelectedGameObject(m_SuperDashButton.gameObject);
			}
			else if(PlayerController.instance.CanSuperDash)
			{
				m_SuperDashButton.interactable = false;
				EventSystem.current.SetSelectedGameObject(m_HeavyHitButton.gameObject);
			}
		}
	}

	//Effects----------------------------------------------------------------------------------------------------------/
	[Header("Effects")]
	[FoldoutGroup("Gameplay Panel")]
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
	[FoldoutGroup("Gameplay Panel")]
	[SerializeField] private GameObject m_WavePanel;
	public GameObject WavePanel
	{
		get
		{
			return m_WavePanel;
		}
	}

	[FoldoutGroup("Gameplay Panel")]
	[SerializeField] private Text m_WaveText;
	public string WaveText
	{
		set
		{
			m_WaveText.text = value;
		}
	}

	[FoldoutGroup("Gameplay Panel")]
	[SerializeField] private Text m_WaveCountdown;
	public Text WaveCountdown
	{
		get
		{
			return m_WaveCountdown;
		}
	}

	#endregion

	#region Pause Panel

//	[FoldoutGroup("Pause Panel")]

	#endregion

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

	#region Methods

	public void SetPanel(Panels panel)
	{
		m_PreviousPanel = m_CurrentPanel;
		if (m_PreviousPanel != null)
		{
			m_PreviousPanel.SetActive(false);
		}

		m_CurrentPanel = m_Panels[panel];
		m_CurrentPanel.SetActive(true);
	}

	public void Resume()
	{
		PlayerController.instance.Paused = false;
	}

	public void Restart()
	{
		SceneLoader.instance.LoadLevel("", "");
		PlayerController.instance.Paused = false;
	}

	#endregion
}
