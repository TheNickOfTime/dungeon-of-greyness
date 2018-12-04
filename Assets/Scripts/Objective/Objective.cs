using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Objective : MonoBehaviour
{
	[SerializeField] protected GenericEvent m_CompletionEvent;
	[SerializeField] protected bool m_EvaluateOnUpdate;
	
	protected abstract bool Condition();

	protected virtual void Update()
	{
		if (m_EvaluateOnUpdate)
		{
			Evaluate();
		}
	}

	protected virtual void Evaluate()
	{
		if (Condition())
		{
			m_CompletionEvent.Invoke();
		}
	}
}
