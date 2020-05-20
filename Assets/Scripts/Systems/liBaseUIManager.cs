using UnityEngine;

/// <summary>
/// Base class for any manager window-like UIs opened inside game
/// </summary>
public abstract class BaseUIManager : MonoBehaviour
{
    /// <summary>
    /// Signals if the UI inside the stack of opened windows.
    /// </summary>
    public bool IsOpen { get; protected set; }

    /// <summary>
    /// Signals if the UI is the top-most of stack and currently visible.
    /// </summary>
    public bool IsMaximized { get; protected set; }

    /// <summary>
    /// Specifies how the window will get opened.
    /// </summary>
    public abstract void OpenUI();

    /// <summary>
    /// Specifies how the window will get closed.
    /// </summary>
    public abstract void CloseUI();

    /// <summary>
    /// Specifies how the window will get minimized.
    /// </summary>
    public abstract void MinimizeUI();

    /// <summary>
    /// Specifies how the window will get maximized.
    /// </summary>
    public abstract void MaximizeUI();
}
