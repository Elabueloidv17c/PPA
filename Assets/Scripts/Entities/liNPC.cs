using UnityEngine;

public class liNPC : MonoBehaviour
{
    /// <summary>
    /// ID of conversation this npc displays.
    /// </summary>
    public int m_conversationID;

    /// <summary>
    /// Signals when player is in range of the npc.
    /// </summary>
    private bool m_playerInRange;

    void Update() 
    {
        // Display conversation when interact key is pressed.
        if(m_playerInRange && 
           !liGameManager.instance.menuActive &&
           Input.GetKeyDown((KeyCode)GameInput.Interact))
        {
            liDialogManager.instance.DisplayConversation(m_conversationID);
        }
    }

    /// <summary>
    /// Register when player enters the range of npc
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            m_playerInRange = true;
        }
    }

    /// <summary>
    /// Register when player is no longer in range of npc
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            m_playerInRange = false;
        }
    }
}
