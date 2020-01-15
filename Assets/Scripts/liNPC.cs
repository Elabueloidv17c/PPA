using UnityEngine;

public class liNPC : MonoBehaviour
{
    public static bool m_isPlayerNearby;

    void Start() {
        m_isPlayerNearby = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_isPlayerNearby = false;
        }
    }
}
