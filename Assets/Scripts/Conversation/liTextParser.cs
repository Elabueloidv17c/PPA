using System.Collections.Generic;
using System.Text.RegularExpressions;

// TODO: Document file

public static class TextParser
{
    public static Regex htmlPatern = new Regex(@"<(?<html>[A-Za-z][A-Za-z0-9]*)\b[^>]*>.*?</\1>");

    public static TextNode[] Parse(string text)
    {
        var result = new List<TextNode>();
        if (htmlPatern.IsMatch(text))
        {
            var match = htmlPatern.Matches(text)[0];

            var head = (new Regex(@"<" + match.Groups["html"] + @"\b[^>]*>")).Match(text);
            var foot = (new Regex(@"</" + match.Groups["html"] + @">")).Match(text);

            //prologue
            result.Add(new TextNode(text.Substring(0, head.Index)));
            //node
            var s = head.Index + head.Length;
            var children = text.Substring(s, foot.Index - s);
            result.Add(new TextNode(head.Value, children, foot.Value));
            //epilogue
            result.AddRange(Parse(text.Substring(foot.Index + foot.Length)));
        }
        else
        {
            result.Add(new TextNode(text));
        }
        return result.ToArray();
    }

    public class TextNode
    {
        public string head;
        public TextNode[] children = null;
        public string foot = null;

        public TextNode(string text)
        {
            this.head = text;
        }

        public TextNode(string head, string children, string foot)
        {
            this.head = head;
            this.children = Parse(children);
            this.foot = foot;
        }
    }
}