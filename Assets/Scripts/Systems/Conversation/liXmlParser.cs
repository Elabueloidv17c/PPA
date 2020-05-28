using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Used to parse strings into xml trees.
/// </summary>
public static class XmlParser
{
    /// <summary>
    /// Regex patern used to find xml elements coupled with their opening and closing tags.
    /// </summary>
    public static Regex xmlPatern = new Regex(@"<(?<xml>[A-Za-z][A-Za-z0-9]*)\b[^>]*>.*?</\1>");

    /// <summary>
    /// Create xml tree from the given string.
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Array of xml tag nodes.</returns>
    public static XmlNode[] Parse(string text)
    {
        var result = new List<XmlNode>();
        if (xmlPatern.IsMatch(text))
        {
            var match = xmlPatern.Matches(text)[0];

            var head = (new Regex(@"<" + match.Groups["xml"] + @"\b[^>]*>")).Match(text);
            var foot = (new Regex(@"</" + match.Groups["xml"] + @">")).Match(text);

            //header
            result.Add(new XmlNode(text.Substring(0, head.Index)));
            //child nodes
            var s = head.Index + head.Length;
            var children = text.Substring(s, foot.Index - s);
            result.Add(new XmlNode(head.Value, children, foot.Value));
            //footer
            result.AddRange(Parse(text.Substring(foot.Index + foot.Length)));
        }
        else
        {
            result.Add(new XmlNode(text));
        }
        return result.ToArray();
    }

    /// <summary>
    /// Represents a node from an xml tree.
    /// </summary>
    public class XmlNode
    {
        /// <summary>
        /// xml header.
        /// </summary>
        public string head;

        /// <summary>
        /// child xml elements
        /// </summary>
        public XmlNode[] children = null;

        /// <summary>
        /// xml footer.
        /// </summary>
        public string foot = null;

        /// <summary>
        /// Create node which contains only a single string text
        /// </summary>
        /// <param name="text">Single text being contained</param>
        public XmlNode(string text)
        {
            this.head = text;
        }

        /// <summary>
        /// Create node representing a full xml element
        /// </summary>
        /// <param name="head"></param>
        /// <param name="children"></param>
        /// <param name="foot"></param>
        public XmlNode(string head, string children, string foot)
        {
            this.head = head;
            this.children = Parse(children);
            this.foot = foot;
        }
    }
}
