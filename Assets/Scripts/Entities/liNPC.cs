﻿using UnityEngine;

public class liNPC : MonoBehaviour
{
    public int m_dialogID;

    private bool m_playerInRange;

    void Start()
    {
    }

    void Update() 
    {
        if(m_playerInRange && 
           !liGameManager.instance.menuActive &&
           Input.GetKeyDown((KeyCode)GameInput.Interact))
        {
            liDialogManager.instance.DisplayDialog(m_dialogID);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            m_playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            m_playerInRange = false;
        }
    }
}
