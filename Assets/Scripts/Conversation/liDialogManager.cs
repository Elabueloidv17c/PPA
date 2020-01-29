using UnityEngine.UI;
using UnityEngine;

public class liDialogManager : MonoBehaviour
{
    public static liDialogManager instance;

    public bool m_startConversation;
    private bool m_isConversationActive;
    public GameObject m_conversationUI;
    private liCharacter m_playerRef;

    Text m_dialog;
    Text m_name;
    Text m_neutralOption;
    Text m_goodOption;
    Text m_badOption;

    void Awake() {
        instance = this;
    }

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

        m_playerRef = GameObject.FindGameObjectWithTag("Player")
                                .GetComponent<liCharacter>();
        
        m_isConversationActive = false;
        m_conversationUI.SetActive(false);
    }

    void Update() {
        if(m_startConversation)
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
            m_startConversation = false;
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

    bool doOnce = true;
    void ConversationLoop()
    {
        if(doOnce)
        {
            var textDialog = liDataManager.m_data.Conversations[0].Dialogs[0].Text;

            m_dialog.GetComponent<liTextTyper>().ShowText(textDialog);

            m_name.text = liDataManager.getCharacterName(liDataManager.m_data.Conversations[0].Character);

            GameObject[] options = GameObject.FindGameObjectsWithTag("Option");
            foreach (GameObject option in options)
            {
                option.SetActive(false);
            }
            if (liDataManager.m_data.Conversations[0].Dialogs[0].Options.Length > 0)
            {
                for (int i = 0; i < liDataManager.m_data.Conversations[0].Dialogs[0].Options.Length; i++)
                {
                    options[i].SetActive(true);
                }
                m_goodOption.text = liDataManager.m_data.Conversations[0].Dialogs[0].Options[0].Text;
                m_badOption.text = liDataManager.m_data.Conversations[0].Dialogs[0].Options[1].Text;
                m_neutralOption.text = liDataManager.m_data.Conversations[0].Dialogs[0].Options[2].Text;
            }
            
            doOnce = false;
        }
    }
}