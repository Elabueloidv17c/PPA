using UnityEngine;

public class liNPC : MonoBehaviour
{
    public int m_dialogID;

    private bool m_playerInRange;
    private liCharacter m_character;

    void Start()
    {
        m_character = FindObjectOfType<liCharacter>();
    }

    void Update() 
    {
        if(m_playerInRange && 
           !m_character.m_isInteracting &&
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
            m_playerInRange = true;
        }
    }
}
