using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

/// <summary>
/// Controls how string of characters are typed into a text box.
/// Meant to generate an rpg-like text effect.
/// </summary>
[RequireComponent(typeof(Text))]
public class liTextTyper : MonoBehaviour
{
    /// <summary>
    /// Signals if the text is currently being typed.
    /// </summary>
    public bool IsTypingText {
        get; private set;
    }
    
    /// <summary>
    /// The speed in characters per second the text is typed at.
    /// </summary>
    public float textSpeed = 50;

    /// <summary>
    /// A copy of the text being typed.
    /// </summary>
    private string textCopy;

    /// <summary>
    /// Text being typed as a parsed xml tree.
    /// </summary>
    private XmlParser.XmlNode[] parsedText;
    
    /// <summary>
    /// Text box being typed to.
    /// </summary>
    private Text textGUI;

    /// <summary>
    /// Stack of closing xml tags
    /// </summary>
    private Stack<string> footerStack;

    void Awake()
    {
        // Initialize text typer.
        textGUI = GetComponent<Text>();
        IsTypingText = false;
    }

    private void Update()
    {
        // When space bar is detected.
        // queue for the next dialog to be shown.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            QueueNextDialog();
        }
    }

    /// <summary>
    /// Ends the dialog typing when typing text.
    /// Otherwise messages dialog manager to display the next dialog.
    /// </summary>
    public void QueueNextDialog()
    {
        if(IsTypingText)
        {
            EndTextTyping();
        }
        else
        {
            liDialogManager.instance.NextDialog();
        }
    }

    /// <summary>
    /// Initializes text typing coroutine.
    /// </summary>
    /// <param name="value">the text to be typed</param>
    public void ShowText(string value)
    {
        StopAllCoroutines();
        textGUI.text = value;
        IsTypingText = true;

        StartCoroutine(TypeText());
    }

    /// <summary>
    /// Main text typing coroutine.
    /// </summary>
    IEnumerator TypeText()
    {
        textCopy = textGUI.text;
        parsedText = XmlParser.Parse(textGUI.text);
        textGUI.text = "";
        footerStack = new Stack<string>();

        foreach (var child in parsedText)
            yield return TypeNode(child);

        EndTextTyping();
    }

    /// <summary>
    /// Coroutine to type individual parsed text node.
    /// </summary>
    IEnumerator TypeNode(XmlParser.XmlNode node)
    {
        if(node.children != null)
        {
            textGUI.text += node.head;
            footerStack.Push(node.foot);
            foreach (var child in node.children)
                yield return TypeNode(child);
            textGUI.text += footerStack.Pop();
        }
        else
        {
            yield return TypeString(node.head);
        }
    }

    /// <summary>
    /// Coroutine to type an actual raw string
    /// </summary>
    IEnumerator TypeString(string text)
    {
        foreach(var c in text)
        {
            textGUI.text += c;
            var localcopy = textGUI.text;
            textGUI.text += string.Join("",footerStack.ToArray());
            yield return new WaitForSeconds(1 / textSpeed);
            textGUI.text = localcopy;
        }
    }

    /// <summary>
    /// Terminate text typing coroutine and inform dialog manager.
    /// </summary>
    public void EndTextTyping()
    {
        StopAllCoroutines();

        textGUI.text = textCopy;
        IsTypingText = false;
        
        liDialogManager.instance.FinishedTyping();
    }
}