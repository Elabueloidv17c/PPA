using System;

/// <summary>
/// Controls how the dialog system works.
/// Use this file to instantiate and reference dialog actions.
/// </summary>
public partial class liDialogManager
{
    /// <summary>
    /// Instance of DialogActionNone
    /// </summary>
    private DialogAction ActionNone;

    /// <summary>
    /// Instance of DialogActionJumpToNext
    /// </summary>
    private DialogAction ActionJumpToNext;

    /// <summary>
    /// Instance of DialogActionButtons
    /// </summary>
    private DialogAction ActionButtons;

    /// <summary>
    /// Instance of DialogActionEnd
    /// </summary>
    private DialogAction ActionEnd;

    /// <summary>
    /// Instance of DialogActionGiveItem
    /// </summary>
    private DialogAction ActionGiveItem;

    /// <summary>
    /// Instantiates DialogActions
    /// </summary>
    void InitializeActions()
    {
        ActionNone = new DialogActionNone();
        ActionJumpToNext = new DialogActionJumpToNext();
        ActionButtons = new DialogActionButtons();
        ActionEnd = new DialogActionEnd();
        ActionGiveItem = new DialogActionGiveItem();
    }

    /// <summary>
    /// Converts a DialogAction enum to a DialogAction instance.
    /// </summary>
    /// <param name="action">Dialog Action enum</param>
    /// <returns>Dialog Action instance</returns>
    DialogAction GetDialogAction(EDialogAction action)
    {
        switch(action)
        {
            case EDialogAction.None: return ActionNone;
            case EDialogAction.JumpToNext: return ActionJumpToNext;
            case EDialogAction.Buttons: return ActionButtons;
            case EDialogAction.End: return ActionEnd;
            case EDialogAction.GiveItem: return ActionGiveItem;
            default: throw new NotImplementedException();
        }
    }
}