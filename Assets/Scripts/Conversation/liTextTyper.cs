using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

[RequireComponent(typeof(Text))]
public class liTextTyper : MonoBehaviour
{
    public bool IsTypingText {
        get; private set;
    }
    
    public float textSpeed = 50;
    private string textCopy;
    private TextParser.TextNode[] parsedText;
    private Text textGUI;
    private Stack<string> footerStack;

    void Awake()
    {
        textGUI = GetComponent<Text>();
        IsTypingText = false;
    }

  private void Update()
  {
    if ((Input.GetKeyDown(KeyCode.Space) || 
         Input.GetMouseButtonDown(0)))
    {
      if(IsTypingText)
      {
        StopAllCoroutines();
        EndTextTyping();
      }
      else
      {
        liDialogManager.instance.NextDialog();
      }
    }
  }

    public void ShowText(string value)
    {
        StopAllCoroutines();
        textGUI.text = value;
        IsTypingText = true;

        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        textCopy = textGUI.text;
        parsedText = TextParser.Parse(textGUI.text);
        textGUI.text = "";
        footerStack = new Stack<string>();

        foreach (var child in parsedText)
            yield return TypeNode(child);

        EndTextTyping();
    }

    IEnumerator TypeNode(TextParser.TextNode node)
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

    void EndTextTyping()
    {
        textGUI.text = textCopy;
        IsTypingText = false;
    }
}