

public class DialogActionEnd : DialogAction
{
    public override bool ClickIntoNextEnabled() => false;

    public override bool ClickToCloseUIEnabled() => true;
}
