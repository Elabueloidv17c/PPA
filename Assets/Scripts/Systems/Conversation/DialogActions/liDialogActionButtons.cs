

public class DialogActionButtons : DialogAction
{
    public override void onFinishedTyping() {
        liDialogManager.instance.ShowButtons();
    }
}
