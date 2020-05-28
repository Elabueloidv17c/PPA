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
    
    #region UI_references
    
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
    
    #endregion

    /// <summary>
    /// ID of the conversation currently being displayed.
    /// </summary>
    private int m_conversationID;

    /// <summary>
    /// Get the list of dialogs for the current conversation.
    /// </summary>
    public Dialog[] CurrentDialogs
    {
        get => liDataManager.m_data.Conversations[m_conversationID].Dialogs;
    }

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
    /// Signals if UI should be closed instead of advancing to the next dialog.
    /// </summary>
    private bool m_clickToCloseUI;

    /// <summary>
    /// Extra Action the dialog manager must execute for the current dialog.
    /// </summary>
    private DialogAction m_action;

    /// <summary>
    /// Contains data as loaded from file about current dialog action.
    /// </summary>
    private LogActionData m_logAction;

    /// <summary>
    /// Contains data as loaded from file about current dialog action.
    /// </summary>
    public LogActionData LogActionData { get => m_logAction; }

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
    /// Opens the dialog manager ui.
    /// </summary>
    public override void OpenUI()
    {
        m_dialogPanel.SetActive(true);
        IsOpen = true;
        IsMaximized = true;

        liGameManager.instance.RegisterOpenUI(this);
    }

    /// <summary>
    /// Closes the dialog manager ui.
    /// </summary>
    public override void CloseUI()
    {
        m_dialogPanel.SetActive(false);
        IsOpen = false;
        IsMaximized = false;

        liGameManager.instance.RegisterCloseUI(this);
    }

    /// <summary>
    /// Hides the dialog manager ui without closing.
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
    /// Unhides the dialog manager ui.
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

        m_conversationID = conversationID;
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
        var dialogs = CurrentDialogs;

        m_textTyper.ShowText(dialogs[m_dialogIndex].Text);

        m_logAction = dialogs[m_dialogIndex].LogActionData;
        m_action = GetDialogAction(m_logAction.ActionType);

        m_clickIntoNext = m_action.ClickIntoNextEnabled();
        m_clickToCloseUI = m_action.ClickToCloseUIEnabled();

        m_action.onDialogBegin();
    }

    /// <summary>
    /// Called by text-typer to advance a conversation from the
    /// current dialog to the next one.
    /// </summary>
    public void NextDialog()
    {
        if(m_clickIntoNext)
        {
            m_dialogIndex = m_action.NextDialogIndex();
            m_action.onDialogEnd();
            DisplayDialog();
            m_elipsis.SetActive(false);
        }
        else if(m_clickToCloseUI)
        {
            CloseUI();
        }
    }

    /// <summary>
    /// Called by text-typer when a dialog is finished typing.
    /// </summary>
    public void FinishedTyping()
    {
        m_action.onFinishedTyping();
        
        if(m_clickIntoNext)
        {
            m_elipsis.SetActive(true);
        }
    }

    /// <summary>
    /// Called by DialogActionButtons when finished typing.
    /// Shows the option buttons according to the current LogOptions.
    /// </summary>
    public void ShowButtons()
    {
        var options = CurrentDialogs[m_dialogIndex].Options;

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

    /// <summary>
    /// Called by the option buttons when the player clicks on any of them.
    /// </summary>
    /// <param name="index">The index of the calling button.</param>
    private void ButtonCallback(int index)
    {
        m_dialogIndex = index;
        DisableButtons();

        if(CurrentDialogs.Length <= m_dialogIndex)
        {
            EndConversation();
        }
        else
        {
            DisplayDialog();
        }

    }

    /// <summary>
    /// Forcibly ends a conversation by closing dialog ui.
    /// </summary>
    private void EndConversation()
    {
        CloseUI();
    }

    /// <summary>
    /// Opens the inventory ui from the dialog ui.
    /// </summary>
    public void OpenInventory()
    {
        liGameManager.instance.OpenInventory();
    }
}
