using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text;

namespace Dialogr
{
    public class Parser
    {
        public static ParsedDialogue Parse(string rawText)
        {
            ParsedDialogue result = new ParsedDialogue();

            SplitLine(rawText, out string rawName, out string rawLine);
            ParseName(rawName, out result.DisplayName, out result.ActorID, out result.DialogueType);
            ParseLine(rawLine, out result.Line, out result.Attributes);

            return result;
        }

        private static void SplitLine(string rawText, out string rawName, out string rawLine)
        {
            rawName = "";

            int indexOfColon = rawText.IndexOf(':');
            if (indexOfColon != -1)
            {
                rawName = rawText.Substring(0, indexOfColon);
                rawName = RemoveLeadingTrailingWhiteSpace(rawName);

                rawLine = rawText.Substring(indexOfColon + 1, rawText.Length - indexOfColon - 1);
                rawLine = RemoveLeadingTrailingWhiteSpace(rawLine);
            }
            else
            {
                rawLine = rawText;
                rawLine = RemoveLeadingTrailingWhiteSpace(rawLine);
            }
        }

        private static void ParseName(
            string rawName, out string displayName, out string actorID, out DialogueType dialogueType)
        {
            // TODO: design a way to distinguish this
            // TODO: then actually implement this
            actorID = rawName;
            displayName = rawName;
            dialogueType = DialogueType.Speech;
        }



        // TODO support callbacks/triggers
        private static void ParseLine(string rawLine, out string trimmedLine, out TextAttribute[] attribs)
        {
            trimmedLine = rawLine;

            string lineCopy = string.Copy(rawLine);
            int removedCount = 0;
            Stack<TextAttribute> stack = new Stack<TextAttribute>();
            List<TextAttribute> textAttributes = new List<TextAttribute>();

            for (int i = 0; i < lineCopy.Length; i++)
            {
                bool found = FindStartingEndingIndex(
                    lineCopy, i, Constants.OpeningBracket, Constants.ClosingBracket,
                    out string modifier, out int length, out bool isOpening);
                if (!found) continue;

                TextAttribute.Type type = TextAttribute.Type.None;
                if (Constants.AttribDef.ContainsKey(modifier))
                    type = Constants.AttribDef[modifier];
                else
                {
                    Debug.LogErrorFormat("unkown attribute named: {0}", modifier);
                    // TODO: figure out if we exit here
                }

                if (isOpening)
                {
                    stack.Push(new TextAttribute
                    {
                        ModType = type,
                        StartingIndex = i - removedCount,
                    });
                }
                else
                {
                    TextAttribute peek = stack.Peek();
                    if (type != peek.ModType)
                    {
                        Debug.LogErrorFormat("mis-matched modifiers <{0}> </{1}>", peek.ModType, type);
                        // TODO: figure out how to handle this error
                    }

                    // Move from stack to list once we know the length.
                    textAttributes.Add(new TextAttribute
                    {
                        ModType = type,
                        StartingIndex = peek.StartingIndex,
                        Length = i - peek.StartingIndex - removedCount
                    });
                    stack.Pop();
                }

                // Trim string
                var countToRemove = 2 + modifier.Length + (isOpening ? 0 : 1);
                trimmedLine = trimmedLine.Remove(i - removedCount, countToRemove);
                removedCount += countToRemove;
                i += length;
            }

            attribs = textAttributes.ToArray();
        }

        // TODO: trim trailing/leading whitespace if it's an issue

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line">The line to analyze.</param>
        /// <param name="index">Index to start searching from.</param>
        /// <param name="startingBracket">Starting character.</param>
        /// <param name="endingBracket">Ending character.</param>
        /// <param name="modifier">The text of the modifier.</param>
        /// <param name="length">Length of the bracket.</param>
        /// <param name="isOpening">Whether this is an opening or ending bracket.</param>
        /// <returns>Whether a starting-ending bracket was found.</returns>
        private static bool FindStartingEndingIndex(string line, int index, char startingBracket, char endingBracket,
            out string modifier, out int length, out bool isOpening)
        {
            length = -1;
            isOpening = false;
            modifier = "";

            if (index < 0 || index >= line.Length)
            {
                Debug.LogErrorFormat("invalid index {0} for line {1}", index, line);
                return false;
            }

            char c = line[index];
            if (c != startingBracket)
            {
                return false;
            }

            int endingIndex = line.IndexOf(endingBracket, index + 1);
            if (endingIndex == -1)
            {
                Debug.LogErrorFormat("found opening bracket, but could not find closing bracket ({0})", line);
                return false;
            }

            isOpening = line[index + 1] != '/'; // if it's <test> or </test>

            length = endingIndex - index - (isOpening ? 0 : 1);
            int startingIndex = index + (isOpening ? 1 : 2);
            modifier = line.Substring(startingIndex, length - 1);

            // Remove all white space.
            // if (modifier.Contains(" "))
            //    modifier = modifier.Replace(" ", "");

            return true;
        }

        /// <summary>
        /// Removes trailing and leading white space from a string.
        /// </summary>
        /// <param name="text">Text to trim.</param>
        /// <returns>Trimmed version of the string.</returns>
        private static string RemoveLeadingTrailingWhiteSpace(string text)
        {
            int removeCount = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (Constants.Space.Contains(text[i]))
                {
                    removeCount++;
                }
                else
                {
                    break;
                }
            }

            if (removeCount == text.Length)
                return "";

            if (removeCount > 0)
                text = text.Substring(removeCount);

            removeCount = 0;
            for (int i = text.Length - 1; i >= 0; i--)
            {
                if (Constants.Space.Contains(text[i]))
                {
                    removeCount++;
                }
                else
                {
                    break;
                }
            }

            if (removeCount > 0)
                text = text.Substring(0, text.Length - removeCount);

            return text;
        }
    }
}