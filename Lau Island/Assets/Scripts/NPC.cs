using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public static bool m_isConversationActive;
    public static bool m_isPlayerNearby;

    public GameObject m_conversationUI;
    Text m_dialog;
    Text m_name;
    Text m_neutralOption;
    Text m_goodOption;
    Text m_badOption;
    public Player m_playerRef;


    void Start() {
        GameObject dialog = GameObject.FindGameObjectWithTag("Dialog");
        m_dialog = dialog.GetComponent<Text>();
        dialog = GameObject.FindGameObjectWithTag("Name");
        m_name = dialog.GetComponent<Text>();
        dialog = GameObject.FindGameObjectWithTag("Good");
        m_goodOption = dialog.GetComponent<Text>();
        dialog = GameObject.FindGameObjectWithTag("Bad");
        m_badOption = dialog.GetComponent<Text>();
        dialog = GameObject.FindGameObjectWithTag("Neutral");
        m_neutralOption = dialog.GetComponent<Text>();
        
        m_isConversationActive = false;
        m_isPlayerNearby = false;
        m_conversationUI.SetActive(false);
    }

    void Update() {
        if(m_isPlayerNearby && !m_isConversationActive)
        {
            EnterConversation();
        }
        if (m_isConversationActive)
        {
            ConversationLoop();
        }
        if (Input.GetKeyDown((KeyCode)GameInput.Exit))
        {
            ExitConversation();
        }
    }

    void EnterConversation()
    {
        if (m_playerRef.m_isInteracting)
        {
            m_isConversationActive = true;
            m_conversationUI.SetActive(true);
        }
    }

    void ExitConversation()
    {
        if (m_isConversationActive)
        {
            m_isConversationActive = false;
            m_playerRef.m_isInteracting = false;
            m_conversationUI.SetActive(false);
        }
    }

    void ConversationLoop()
    {
        m_dialog.text = DataManager.m_data.Conversations[0].Dialogs[0].Text;
        m_name.text = DataManager.getCharacterName(DataManager.m_data.Conversations[0].Character);

        GameObject[] options = GameObject.FindGameObjectsWithTag("Option");
        foreach (GameObject option in options)
        {
            option.SetActive(false);
        }
        if (DataManager.m_data.Conversations[0].Dialogs[0].Options.Length > 0)
        {
            for (int i = 0; i < DataManager.m_data.Conversations[0].Dialogs[0].Options.Length; i++)
            {
                options[i].SetActive(true);
            }
            m_goodOption.text = DataManager.m_data.Conversations[0].Dialogs[0].Options[0].Text;
            m_badOption.text = DataManager.m_data.Conversations[0].Dialogs[0].Options[1].Text;
            m_neutralOption.text = DataManager.m_data.Conversations[0].Dialogs[0].Options[2].Text;
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
