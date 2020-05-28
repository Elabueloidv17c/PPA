

public class DialogActionJumpToNext : DialogAction
{
    public override bool ClickIntoNextEnabled() 
    {
        return liDialogManager.instance.LogActionData.Next < 
               liDialogManager.instance.CurrentDialogs.Length;
    }

    public override int NextDialogIndex()
    { 
        return liDialogManager.instance.LogActionData.Next; 
    }
}
