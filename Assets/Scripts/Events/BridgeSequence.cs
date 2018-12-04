using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BridgeSequence : MonoBehaviour
{
	[SerializeField] private Tilemap m_Tilemap;
	[SerializeField] private RuleTile m_Tile;
	[Space]
	[SerializeField] private bool m_IsVisibleOnStart = false;
	[Space]
	[SerializeField] private AudioClip m_SFX;

	private Transform m_Bridge;

	private void Awake()
	{
		m_Bridge = transform;

		for (int i = 0; i < m_Bridge.childCount; i++)
		{
			m_Bridge.GetChild(i).gameObject.SetActive(m_IsVisibleOnStart);
			
			Vector3Int pos = m_Tilemap.WorldToCell(m_Bridge.GetChild(i).position);
			RuleTile tile = m_IsVisibleOnStart ? null : m_Tile;
			m_Tilemap.SetTile(pos, tile);
		}
	}

	public void LowerBridge()
	{
		StartCoroutine(LowerBridgeSequence());
	}

	public void RaiseBridge()
	{
		StartCoroutine(RaiseBridgeSequence());
	}

	private IEnumerator LowerBridgeSequence()
	{
		for (int i = m_Bridge.childCount - 1; i > 0; i -= 2)
		{
			
			
			Vector3Int pos = m_Tilemap.WorldToCell(m_Bridge.GetChild(i).position);
			m_Tilemap.SetTile(pos, m_Tile);
			
			SFXManager.PlayClipAtPoint(m_SFX, new Vector2(pos.x, pos.y));
			
			pos = m_Tilemap.WorldToCell(m_Bridge.GetChild(i - 1).position);
			m_Tilemap.SetTile(pos, m_Tile);
			
			yield return new WaitForSeconds(0.05f);
			
			
			m_Bridge.GetChild(i).gameObject.SetActive(false);
			m_Bridge.GetChild(i - 1).gameObject.SetActive(false);
			
			Shaker.instance.CameraShake(0.5f, 1.35f, 0.1f);

			yield return new WaitForSeconds(0.1f);
		}
	}

	private IEnumerator RaiseBridgeSequence()
	{
		for (int i = 0; i < m_Bridge.childCount; i += 2)
		{
			Vector3Int pos = m_Tilemap.WorldToCell(m_Bridge.GetChild(i).position);
			SFXManager.PlayClipAtPoint(m_SFX, new Vector2(pos.x, pos.y));

			m_Bridge.GetChild(i).gameObject.SetActive(true);
			m_Bridge.GetChild(i + 1).gameObject.SetActive(true);

			yield return new WaitForSeconds(0.05f);

			m_Tilemap.SetTile(pos, null);
			
			pos = m_Tilemap.WorldToCell(m_Bridge.GetChild(i + 1).position);
			m_Tilemap.SetTile(pos, null);
			
			Shaker.instance.CameraShake(0.75f, 1.35f, 0.1f);
			
			yield return new WaitForSeconds(0.1f);
		}
	}
}
