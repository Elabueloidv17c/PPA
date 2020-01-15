using UnityEngine;

public class liNPC : MonoBehaviour
{
    public static bool m_isPlayerNearby;
    private liCharacter m_playerRef;

    void Start() {
        m_isPlayerNearby = false;
        
        m_playerRef = GameObject.FindGameObjectWithTag("Player")
                                .GetComponent<liCharacter>();
    }

    void Update() {
        if(m_isPlayerNearby && m_playerRef.m_isInteracting) {
            liDialogManager.instance.m_startConversation = true;
        }
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
