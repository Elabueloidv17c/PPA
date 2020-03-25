using System;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;
using Button = UnityEngine.UI.Button;

// TODO: Document file

public class liDialogManager : BaseUIManager
{
    public static liDialogManager instance;
    private liPlayerCharacter m_character;
    
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
        m_character = FindObjectOfType<liPlayerCharacter>();
        m_dialogPanel = transform.GetChild(0).gameObject;

        var bkgd = m_dialogPanel.transform.Find("Background");

        m_textTyper = bkgd.GetComponentInChildren<liTextTyper>();

        m_charNameText = bkgd.Find("Character Name").
                         GetComponentInChildren<Text>();

        m_elipsis = bkgd.Find("Elipsis").gameObject;
        m_elipsis.SetActive(false);

        m_buttons = m_dialogPanel.transform.Find("Buttons Panel").
                    GetComponentsInChildren<Button>();
        
        DisableButtons();

        CloseUI();
    }

    public override void OpenUI()
    {
        m_dialogPanel.SetActive(true);
        IsOpen = true;
        IsMaximized = true;

        liGameManager.instance.RegisterOpenUI(this);
    }

    public override void CloseUI()
    {
        m_dialogPanel.SetActive(false);
        IsOpen = false;
        IsMaximized = false;

        liGameManager.instance.RegisterCloseUI(this);
    }

    public override void MinimizeUI()
    {
        if(!IsOpen) { return; }

        if(m_textTyper.IsTypingText) { 
            m_textTyper.EndTextTyping();
        }

        IsMaximized = false;
        m_dialogPanel.SetActive(false);
    }

    public override void MaximizeUI()
    {
        if(!IsOpen) { return; }
        IsMaximized = true;
        m_dialogPanel.SetActive(true);
    }


    void DisableButtons()
    {
        Array.ForEach(m_buttons, b => b.gameObject.SetActive(false));
    }

    public void DisplayDialog(int dialogID)
    {
        OpenUI();

        m_dialogID = dialogID;
        m_dialogIndex = 0;

        m_charNameText.text = 
            liDataManager.m_data.Conversations[dialogID].
                Character.ToString().Replace("_", " ");

        DisplayIndividualDialog();
    }

    private void DisplayIndividualDialog()
    {
        var dialogs = liDataManager.m_data.Conversations[m_dialogID].Dialogs;

        m_textTyper.ShowText(dialogs[m_dialogIndex].Text);

        action = dialogs[m_dialogIndex].LogAction;

        switch (action.ActionType)
        {
            case ActionType.None:
                m_nextEnabled = m_dialogIndex + 1 < dialogs.Length;
            break;
            case ActionType.JumpToNext:
                m_nextEnabled = action.Next < dialogs.Length;
            break;
            case ActionType.Buttons:
                m_nextEnabled = false;
            break;
            case ActionType.End:
                m_nextEnabled = false;
            break;
            case ActionType.GiveItem:
                m_nextEnabled = m_dialogIndex + 1 < dialogs.Length;
                liInventory.instance.AddItem(action.Value);
            break;
            
            default:
            throw new NotImplementedException();
        }
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
            CloseUI();
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
        CloseUI();
    }

    public void OpenInventory()
    {
        liGameManager.instance.OpenInventory();
    }
}