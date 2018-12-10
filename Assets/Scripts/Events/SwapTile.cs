using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SwapTile : MonoBehaviour
{
	private Transform[,] m_Coordinates;

	[SerializeField] private Tilemap m_TilemapA;
	[SerializeField] private RuleTile m_TileA;
	[Space]
	[SerializeField] private Tilemap m_TilemapB;
	[SerializeField] private RuleTile m_TileB;
	[Space]
	[SerializeField] private AudioClip m_AudioClip;
	
	public enum Direction
	{
		Forward,
		Reverse
	}

	private void Awake()
	{
		m_Coordinates = new Transform[transform.childCount, 20];
		for (int i = 0; i < transform.childCount; i++)
		{
			for (int j = 0; j < transform.GetChild(i).childCount; j++)
			{
				m_Coordinates[i, j] = transform.GetChild(i).GetChild(j);
			}
		}
	}

	public void SwapSpritesForward()
	{
		StartCoroutine(SwapSequence(true));
	}

	public void SwapSpritesReverse()
	{
		StartCoroutine(SwapSequence(false));
	}

	private IEnumerator SwapSequence(bool forward)
	{
		for (int i = forward ? 0 : transform.childCount; forward ? i < transform.childCount : i > 0; i += forward ? 1 : -1)
		{
			for (int j = forward ? 0 : transform.GetChild(i).childCount; forward ? j < transform.GetChild(i).childCount : i > 0; j += forward ? 1 : -1)
			{
				Vector3Int pos = m_TilemapA.WorldToCell(m_Coordinates[i, j].position);
				m_TilemapA.SetTile(pos, m_TileA);

				pos = m_TilemapB.WorldToCell(m_Coordinates[i, j].position);
				m_TilemapB.SetTile(pos, m_TileB);
			}
			SFXManager.PlayClipAtPoint(m_AudioClip, transform.position);
			yield return new WaitForSeconds(0.1f);
		}
	}
}
