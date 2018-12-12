using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorHazard : MonoBehaviour
{
	public void Damage()
	{
		Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(2,2), 0);

		for (int i = 0; i < hits.Length; i++)
		{
			PlayerController player = hits[i].GetComponent<PlayerController>();
			if (player != null)
			{
				player.OnHit(Vector2.zero, 1);
			}
		}
	}

	public void PlayAudio()
	{
		GetComponent<AudioSource>().Play();
	}

	public void Destroy()
	{
		Destroy(gameObject);
	}
}
