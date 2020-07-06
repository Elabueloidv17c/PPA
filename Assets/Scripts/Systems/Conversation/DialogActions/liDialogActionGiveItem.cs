

public class DialogActionGiveItem : DialogAction
{
    public override void onDialogBegin() {
        liInventory.instance.AddItem(
            liDialogManager.instance.LogActionData.Value);
    }
}
