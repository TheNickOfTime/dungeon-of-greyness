using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrder : MonoBehaviour
{
	private SpriteRenderer[] m_DynamicSprites;
	
	private void Start()
	{
		List<SpriteRenderer> allSprites = new List<SpriteRenderer>();
		allSprites.AddRange(FindObjectsOfType<SpriteRenderer>());
		
		for (int i = 0; i < allSprites.Count; i++)
		{
			//allSprites[i].sortingOrder =  -(int)(allSprites[i].bounds.center.y - allSprites[i].bounds.extents.y);
			Sort(allSprites[i]);
			if (allSprites[i].gameObject.isStatic)
			{
				allSprites.RemoveAt(i);
			}
		}

		m_DynamicSprites = allSprites.ToArray();
	}

	private void Update()
	{
		for (int i = 0; i < m_DynamicSprites.Length; i++)
		{
			Sort(m_DynamicSprites[i]);
		}
	}

	private void Sort(SpriteRenderer obj)
	{
		if (obj == null) return;
		
		obj.sortingOrder = -(int)(obj.transform.position.y * 10);
	}
}
