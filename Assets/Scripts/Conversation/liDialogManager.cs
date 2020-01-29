using UnityEngine.UI;
using UnityEngine;

public class liDialogManager : MonoBehaviour
{
    public static liDialogManager instance;
    private liCharacter m_character;
    private GameObject m_dialogPanel;
    private liTextTyper m_textTyper;
    private Text m_charNameText;
    private int m_dialogID;
    private int m_dialogIndex;

    void Awake() 
    {
        instance = this;
    }

    void Start() 
    {
        m_character = FindObjectOfType<liCharacter>();
        m_dialogPanel = transform.GetChild(0).gameObject;

        m_textTyper =
            m_dialogPanel.GetComponentInChildren<liTextTyper>();

        m_dialogPanel.SetActive(false);
    }

    public void DisplayDialog(int dialogID)
    {
        m_character.m_isInteracting = true;

        m_dialogID = dialogID;
        m_dialogIndex = 0;

        m_charNameText.text = liDataManager.getCharacterName(
            liDataManager.m_data.Conversations[dialogID].
            Character
        );

        DisplayIndividualDialog();
    }

    private void DisplayIndividualDialog()
    {
        m_textTyper.ShowText(
            liDataManager.m_data.Conversations[m_dialogID].
            Dialogs[m_dialogIndex].Text
        );
    }

    public void NextDialog()
    {
        if()
        {

        }
        else
        {
            m_dialogPanel.SetActive(false);
        }
    }
}