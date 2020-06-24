using UnityEngine;

public class liItemDeposit : MonoBehaviour
{
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
            liInventory.instance.OpenUIDepositMode();
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
