using System;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;
using Button = UnityEngine.UI.Button;

/// <summary>
/// Controls how the dialog system works.
/// Also represents the actual dialog window.
/// </summary>
public partial class liDialogManager : BaseUIManager
{
    /// <summary>
    /// Singleton-like dialog manager instance.
    /// </summary>
    public static liDialogManager instance;

    /// <summary>
    /// reference to player character.
    /// </summary>
    private liPlayerCharacter m_character;
    
    /// <summary>
    /// reference to main dialog panel
    /// </summary>
    private GameObject m_dialogPanel;

    /// <summary>
    /// reference to text typer
    /// </summary>
    private liTextTyper m_textTyper;

    /// <summary>
    /// text box of the character who's saying the dialog.
    /// </summary>
    private Text m_charNameText;

    /// <summary>
    /// Buttons used when giving options to the player.
    /// </summary>
    private Button[] m_buttons;

    /// <summary>
    /// Elipsis icon used to tell the player
    /// that they can click to show the next dialog.
    /// </summary>
    private GameObject m_elipsis;

    /// <summary>
    /// ID of the conversation currently being displayed.
    /// </summary>
    private int m_convID;

    /// <summary>
    /// Index of the dialog currently being displayed.
    /// </summary>
    private int m_dialogIndex;

    /// <summary>
    /// Gets index of the dialog currently being displayed.
    /// </summary>
    public int DialogIndex { get => m_dialogIndex; }

    /// <summary>
    /// Signals the player can click to show the next dialog.
    /// </summary>
    private bool m_clickIntoNext;

    /// <summary>
    /// Extra Action the dialog manager must execute for the current dialog.
    /// </summary>
    private LogAction action;

    void Awake() 
    {
        // Initialize dialog manager internals
        instance = this;
        InitializeActions();
    }

    void Start() 
    {
        // Get and initialize dialog manager external references

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

    /// <summary>
    /// Opens the dialog manager UI.
    /// </summary>
    public override void OpenUI()
    {
        m_dialogPanel.SetActive(true);
        IsOpen = true;
        IsMaximized = true;

        liGameManager.instance.RegisterOpenUI(this);
    }

    /// <summary>
    /// Closes the dialog manager UI.
    /// </summary>
    public override void CloseUI()
    {
        m_dialogPanel.SetActive(false);
        IsOpen = false;
        IsMaximized = false;

        liGameManager.instance.RegisterCloseUI(this);
    }

    /// <summary>
    /// Hides the dialog manager UI without closing.
    /// </summary>
    public override void MinimizeUI()
    {
        if(!IsOpen) { return; }

        if(m_textTyper.IsTypingText) { 
            m_textTyper.EndTextTyping();
        }

        IsMaximized = false;
        m_dialogPanel.SetActive(false);
    }

    /// <summary>
    /// Unhides the dialog manager UI.
    /// </summary>
    public override void MaximizeUI()
    {
        if(!IsOpen) { return; }
        IsMaximized = true;
        m_dialogPanel.SetActive(true);
    }

    /// <summary>
    /// Disable all the player option buttons.
    /// </summary>
    void DisableButtons()
    {
        Array.ForEach(m_buttons, b => b.gameObject.SetActive(false));
    }

    /// <summary>
    /// Main entry point for dialog manager to start a conversation.
    /// </summary>
    /// <param name="conversationID">ID of conversation to be displayed.</param>
    public void DisplayConversation(int conversationID)
    {
        OpenUI();

        m_convID = conversationID;
        m_dialogIndex = 0;

        m_charNameText.text = 
            liDataManager.m_data.Conversations[conversationID].
                Character.ToString().Replace("_", " ");

        DisplayDialog();
    }

    /// <summary>
    /// Display the next individual dialog from a conversation.
    /// </summary>
    private void DisplayDialog()
    {
        var dialogs = liDataManager.m_data.Conversations[m_convID].Dialogs;

        m_textTyper.ShowText(dialogs[m_dialogIndex].Text);

        action = dialogs[m_dialogIndex].Action;

        switch (action.ActionType)
        {
            case EDialogAction.None:
                m_clickIntoNext = m_dialogIndex + 1 < dialogs.Length;
            break;
            case EDialogAction.JumpToNext:
                m_clickIntoNext = action.Next < dialogs.Length;
            break;
            case EDialogAction.Buttons:
                m_clickIntoNext = false;
            break;
            case EDialogAction.End:
                m_clickIntoNext = false;
            break;
            case EDialogAction.GiveItem:
                m_clickIntoNext = m_dialogIndex + 1 < dialogs.Length;
                liInventory.instance.AddItem(action.Value);
            break;
            
            default:
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Called by text-typer to advance a conversation from the
    /// current dialog to the next one.
    /// </summary>
    public void NextDialog()
    {
        if(action.ActionType == EDialogAction.Buttons)
        {
            return;
        }
        else if(m_clickIntoNext)
        {
            if(action.ActionType == EDialogAction.JumpToNext)
            {
                m_dialogIndex = action.Next;
            }
            else
            {
                m_dialogIndex++;
            }
            
            DisplayDialog();
            m_elipsis.SetActive(false);
        }
        else
        {
            CloseUI();
        }
    }

    /// <summary>
    /// Called by text-typer when a dialog is finished typing.
    /// </summary>
    public void FinishedTyping()
    {
        if(action.ActionType == EDialogAction.Buttons)
        {
            var options = liDataManager.m_data.Conversations[m_convID].
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
        
        if(m_clickIntoNext)
        {
            m_elipsis.SetActive(true);
        }
    }

    /// <summary>
    /// Called by the option buttons when the player clicks on any of them.
    /// </summary>
    /// <param name="index">The index of the calling button.</param>
    private void ButtonCallback(int index)
    {
        m_dialogIndex = index;
        DisableButtons();

        if(liDataManager.m_data.Conversations[m_convID].
           Dialogs.Length <= m_dialogIndex)
        {
            EndConversation();
        }
        else
        {
            DisplayDialog();
        }

    }

    /// <summary>
    /// Forcibly ends a conversation by closing dialog UI.
    /// </summary>
    private void EndConversation()
    {
        CloseUI();
    }

    /// <summary>
    /// Opens the inventory UI from the dialog UI.
    /// </summary>
    public void OpenInventory()
    {
        liGameManager.instance.OpenInventory();
    }
}
