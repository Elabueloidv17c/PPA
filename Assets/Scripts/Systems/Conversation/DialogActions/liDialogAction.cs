
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// Contains data as loaded from file about a dialog action.
/// </summary>
public struct LogActionData
{
    public EDialogAction ActionType;
    public int Next;
    public int Value;
}

/// <summary>
/// Enum representation of a dialog action.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]  
public enum EDialogAction {
    None,
    JumpToNext,
    Buttons,
    End,
    GiveItem,

    //... 
}

/// <summary>
/// An extra action a Dialog can perform on the DialogManager.
/// </summary>
public abstract class DialogAction
{
    /// <summary>
    /// Additional actions to perform right as
    /// DialogAction becomes the current action.
    /// </summary>
    public virtual void onDialogBegin() {}

    /// <summary>
    /// Provides a statement which enables jumping to
    /// the next dialog when the user clicks to skip dialog.
    /// </summary>
    public virtual bool ClickIntoNextEnabled() 
    {
        return liDialogManager.instance.DialogIndex + 1 < 
               liDialogManager.instance.CurrentDialogs.Length; 
    }

    /// <summary>
    /// The index of the next dialog to jump to when
    /// ClickIntoNextEnabled returns true.
    /// </summary>
    public virtual int NextDialogIndex() 
    { 
        return liDialogManager.instance.DialogIndex + 1; 
    }

    /// <summary>
    /// Provides a statement which enables closing UI
    /// when the user clicks to skip dialog.
    /// If ClickIntoNextEnabled returns true, this option is ignored.
    /// </summary>
    public virtual bool ClickToCloseUIEnabled() => false;

    /// <summary>
    /// Additional actions to perform when text typer is finished typing.
    /// </summary>
    public virtual void onFinishedTyping() {}

    /// <summary>
    /// Additional actions to perform right before
    /// DialogAction stops being the current action.
    /// </summary>
    public virtual void onDialogEnd() {}
}

public class DialogActionNone : DialogAction {}
