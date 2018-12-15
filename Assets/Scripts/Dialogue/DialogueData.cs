using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue_", menuName = "Dialogue/DialogueData", order = 0)]
public class DialogueData : ScriptableObject
{
	[System.Serializable]
	private struct DialogueLine
	{
		public string m_Line;
	}

	[TextArea]
	public string[] m_Lines;
}
