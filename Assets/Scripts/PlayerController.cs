using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Character m_Char;

	//Values-----------------------------------------------------------------------------------------------------------/
	private Vector2 m_LastDir = new Vector2(0, -1);

    private void Awake()
    {
        m_Char = GetComponent<Character>();
    }

	private void Update()
	{
		PlayerInput();
	}

	private void PlayerInput()
	{
		#region Movement

		//Get Values
		float h = Input.GetButtonDown("Vertical") ? 0 : Input.GetAxis("Horizontal");
		float v = Input.GetButtonDown("Horizontal") ? 0 : Input.GetAxis("Vertical");
		bool isMoving = h != 0 || v != 0;
		
		//Track Values
		if (isMoving)
		{
			m_LastDir = new Vector2(h, v);
			m_LastDir = Vector3.Normalize(m_LastDir);
		}
		
		//Use Values
		m_Char.Move(Vector3.Normalize(new Vector2(h, v)));
		
		m_Char.Anim.SetFloat("Horizontal", h);
		m_Char.Anim.SetFloat("Vertical", v);
		m_Char.Anim.SetBool("IsMoving", isMoving);
		m_Char.Anim.SetFloat("Idle_Horizontal", m_LastDir.x);
		m_Char.Anim.SetFloat("Idle_Vertical", m_LastDir.y);

		#endregion

		#region Attack

		if (Input.GetButtonDown("Attack"))
		{
			m_Char.Attack();
		}

		#endregion
	}
}
