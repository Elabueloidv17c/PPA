using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextTyper : MonoBehaviour
{
    public bool IsTypingText
    {
        get;
        private set;
    }
    public int textSpeed = 50;
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
    if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
    {
      if(IsTypingText)
      {
        StopAllCoroutines();
        EndTextTyping();
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
            yield return new WaitForSecondsRealtime(1 / textSpeed);
            textGUI.text = localcopy;
        }
    }

    void EndTextTyping()
    {
        textGUI.text = textCopy;
        IsTypingText = false;
        /*switch (textOption)
        {
          case DialogTextOption.EndWithArrow:
              Arrow.SetActive(true);
              break;
          case DialogTextOption.EndWithoutArrow:
            Arrow.SetActive(false);
            break;
          case DialogTextOption.EndWithOptions:
              SendMessageUpwards("ShowButtons");
              break;
          default:
              break;
        }*/
    }
}