using System;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;
using Button = UnityEngine.UI.Button;

// TODO: Document file

public class liDialogManager : MonoBehaviour
{
    public static liDialogManager instance;
    private liCharacter m_character;
    
    private GameObject m_dialogPanel;
    private liTextTyper m_textTyper;
    private Text m_charNameText;
    private Button[] m_buttons;

    private GameObject m_elipsis;

    private int m_dialogID;
    private int m_dialogIndex;
    private bool m_nextEnabled;

    private LogAction action;

    void Awake() 
    {
        instance = this;
    }

    void Start() 
    {
        m_character = FindObjectOfType<liCharacter>();
        m_dialogPanel = transform.GetChild(0).gameObject;

        var bkgd = m_dialogPanel.transform.Find("Background");

        m_textTyper = bkgd.GetComponentInChildren<liTextTyper>();

        m_charNameText = bkgd.Find("Character Name").
                         GetComponentInChildren<Text>();

        m_elipsis = bkgd.Find("Elipsis").gameObject;
        m_elipsis.SetActive(false);

        m_buttons = m_dialogPanel.transform.Find("Buttons Panel")
            .GetComponentsInChildren<Button>();
        
        DisableButtons();

        m_dialogPanel.SetActive(false);
    }

    void DisableButtons()
    {
        Array.ForEach(m_buttons, b => b.gameObject.SetActive(false));
    }

    public void DisplayDialog(int dialogID)
    {
        m_dialogPanel.SetActive(true);

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
        var dialogs = liDataManager.m_data.Conversations[m_dialogID].Dialogs;

        m_textTyper.ShowText(dialogs[m_dialogIndex].Text);

        action = dialogs[m_dialogIndex].LogAction;

        m_nextEnabled = (action.ActionType == ActionType.None) 
                        ? m_dialogIndex + 1 < dialogs.Length :
                        (action.ActionType == ActionType.JumpToNext) 
                        ? action.Next < dialogs.Length : false;
    }

    public void NextDialog()
    {
        if(action.ActionType == ActionType.Buttons)
        {
            return;
        }
        else if(m_nextEnabled)
        {
            if(action.ActionType == ActionType.JumpToNext)
            {
                m_dialogIndex = action.Next;
            }
            else
            {
                m_dialogIndex++;
            }
            
            DisplayIndividualDialog();
            m_elipsis.SetActive(false);
        }
        else
        {
            m_dialogPanel.SetActive(false);
            m_character.m_isInteracting = false;
        }
    }

    public void FinishedTyping()
    {
        if(action.ActionType == ActionType.Buttons)
        {
            var options = liDataManager.m_data.Conversations[m_dialogID].
                          Dialogs[m_dialogIndex].Options;

            for(int i = 0; i < options.Length; ++i)
            {
                m_buttons[i].gameObject.SetActive(true);
                
                m_buttons[i].GetComponentInChildren<Text>()
                            .text = options[i].Text;

                // nextIndex forces lambda capture by value
                int nextIndex = options[i].Next;

                m_buttons[i].onClick.AddListener(
                    () => ButtonCallback(nextIndex));
            }
        }
        else if(m_nextEnabled)
        {
            m_elipsis.SetActive(true);
        }
    }

    private void ButtonCallback(int index)
    {
        m_dialogIndex = index;
        DisableButtons();

        if(liDataManager.m_data.Conversations[m_dialogID].
           Dialogs.Length <= m_dialogIndex)
        {
            EndDialog();
        }
        else
        {
            DisplayIndividualDialog();
        }

    }

    private void EndDialog()
    {
        m_dialogPanel.SetActive(false);
        m_character.m_isInteracting = false;
    }
}