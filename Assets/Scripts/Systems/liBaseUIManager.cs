using UnityEngine;

public abstract class BaseUIManager : MonoBehaviour
{
    public bool IsOpen { get; protected set; }

    public bool IsMaximized { get; protected set; }

    public abstract void OpenUI();

    public abstract void CloseUI();

    public abstract void MinimizeUI();

    public abstract void MaximizeUI();
}
