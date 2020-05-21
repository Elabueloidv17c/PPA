
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// Contains data as loaded from file about a dialog action.
/// </summary>
public struct LogAction
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

public abstract class DialogAction
{
    public virtual void onDialogBegin() {}
    public virtual int NextDialogIndex() 
    { 
        return liDialogManager.instance.DialogIndex + 1; 
    }
    public virtual bool ClickIntoNextEnabled() { return true; }

    public virtual void onDialogEnd() {}
}