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

            rawText = CleanLine(rawText);

            SplitLine(rawText, out string rawName, out string rawLine);
            ParseName(rawName, out result.DisplayName, out result.ActorID);
            ParseLine(rawLine, out result.Triggers, out result.Line);

            return result;
        }
        private static string CleanLine(string rawText)
        {
            Debug.LogFormat(" Pre-cleaning=({0})", rawText);
            // Remove trailing, leading spaces
            rawText = RemoveLeadingTrailingWhiteSpace(rawText);
            // Remove spaces surrounding ':'
            rawText = RemoveSurroundingSpace(rawText, rawText.IndexOf(':'));
            rawText = RemoveSurroundingSpace(rawText, rawText.IndexOf('/')); // </ shake> -> </shake>
            // Remove space before '<' and one after
            rawText = RemoveOneSurrounding(rawText, '<', -1);
            // Remove space after '>' and one before
            rawText = RemoveOneSurrounding(rawText, '>', 1);
            // Remove extra spaces within <>
            rawText = RemoveExtraSpace(rawText, '<', '>');
            Debug.LogFormat("Post-cleaning=({0})", rawText);
            return rawText;
        }

        private static string RemoveExtraSpace(string text, char opening, char closing)
        {
            for (int i = 0; i < text.Length; i++)
            {
                string subString = text.Substring(i);
                int startIndex = subString.IndexOf(opening);
                if (startIndex == -1) break;
                int endIndex = subString.IndexOf(closing);

                text = RemoveExtraSpace(text, startIndex + i, endIndex + i, out int removed);

                i += endIndex - removed;
            }

            return text;
        }

        private static string RemoveExtraSpace(string text, int startingIndex, int closingIndex, out int removedCount)
        {
            removedCount = 0;
            bool prevWasSpace = false;
            for (int i = startingIndex; i < closingIndex; i++)
            {
                bool currIsSpace = IndexIsSpace(i, text);
                if (prevWasSpace && currIsSpace)
                {
                    text = text.Remove(i, 1);
                    i--;
                    removedCount++;
                }
                else
                {
                    prevWasSpace = currIsSpace;
                }
            }

            return text;
        }

        private static string RemoveOneSurrounding(string text, char c, int offset)
        {
            for (int i = 0; i < text.Length; i++)
            {
                int matchIndex = text.Substring(i).IndexOf(c);
                if (matchIndex == -1) break;

                i += matchIndex;
                int removed = 0;

                if (IndexIsSpace(i + offset, text))
                {
                    text = text.Remove(i + offset, 1);
                    if (offset == -1) i--;
                    removed++;
                }

                if (offset == 1)
                {
                    text = RemoveSurroundingSpaceLeft(text, i, out int removedLeft);
                    removed += removedLeft;
                }
                else if (offset == -1)
                {
                    text = RemoveSurroundingSpaceRight(text, i, out int removedRight);
                    removed += removedRight;
                }

                i -= removed;
            }

            return text;
        }

        private static bool IndexIsSpace(int index, string text)
        {
            return index >= 0 && index < text.Length && Constants.Space.Contains(text[index]);
        }

        private static string RemoveSurroundingSpace(string text, int index)
        {
            if (index == -1) return text;
            int preIndex = index - 1;
            int count = 0;
            while (IndexIsSpace(preIndex, text))
            {
                preIndex--;
                count++;
            }
            if (count > 0)
            {
                // Debug.LogFormat("Removing pre substring: ({0})", text.Substring(preIndex + 1, count));
                text = text.Remove(preIndex + 1, count);
            }

            int shiftedIndex = index - count;
            int postIndex = shiftedIndex + 1;
            count = 0;
            while (IndexIsSpace(postIndex, text))
            {
                postIndex++;
                count++;
            }
            if (count > 0)
            {
                // Debug.LogFormat("Removing post substring: ({0})", text.Substring(shiftedIndex + 1, count));
                text = text.Remove(shiftedIndex + 1, count);
            }

            return text;
        }

        private static string RemoveSurroundingSpaceLeft(string text, int index, out int removed)
        {
            removed = 0;
            if (index == -1) return text;
            int preIndex = index - 1;
            while (IndexIsSpace(preIndex, text))
            {
                preIndex--;
                removed++;
            }
            if (removed > 0)
            {
                // Debug.LogFormat("Removing pre substring: ({0})", text.Substring(preIndex + 1, removed));
                text = text.Remove(preIndex + 1, removed);
            }

            return text;
        }

        private static string RemoveSurroundingSpaceRight(string text, int index, out int removed)
        {
            removed = 0;
            if (index == -1) return text;
            int postIndex = index + 1;
            while (IndexIsSpace(postIndex, text))
            {
                postIndex++;
                removed++;
            }
            if (removed > 0)
            {
                // Debug.LogFormat("Removing post substring: ({0})", text.Substring(index + 1, removed));
                text = text.Remove(index + 1, removed);
            }


            return text;
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
            string rawName, out string displayName, out string actorID)
        {
            actorID = rawName;
            displayName = rawName;

            //  <bob> Mystery Man: Test Dialogue
            //      ID          = bob
            //      displayName = Mystery Man 

            int startingIndex = rawName.IndexOf(Constants.OpeningBracket);
            if (startingIndex == -1)
            {
                return;
            }

            bool foundID = FindStartingEndingIndex(
                rawName, startingIndex, Constants.OpeningBracket, Constants.ClosingBracket,
                out string id, out int length, out bool _);
            if (!foundID)
            {
                Debug.LogErrorFormat("Misformated name: {0}", rawName);
                actorID = "";
                return;
            }

            actorID = id;
            displayName = rawName.Remove(startingIndex, length);
            // actorID = RemoveLeadingTrailingWhiteSpace(id);
            // displayName = RemoveLeadingTrailingWhiteSpace(rawName.Remove(startingIndex, length));
        }

        // TODO support callbacks/triggers
        private static void ParseLine(string rawLine, out ParsedDialogueTrigger[] triggers, out ParsedLine line)
        {
            triggers = null;
            line = new ParsedLine();
            line.Text = rawLine;

            string lineCopy = string.Copy(rawLine);
            int removedCount = 0;
            int inserted = 0;
            List<ParsedDialogueTrigger> stagedTriggers = new List<ParsedDialogueTrigger>();
            List<ParsedDialogueTrigger> parsedTriggers = new List<ParsedDialogueTrigger>();

            for (int i = 0; i < lineCopy.Length; i++)
            {
                // bool found = FindStartingEndingIndex(
                //     lineCopy, i, Constants.OpeningBracket, Constants.ClosingBracket,
                //     out string modifier, out int length, out bool isOpening);
                // if (!found) continue;

                // Skip until next '<'
                int indexOfNextOpening = lineCopy.IndexOf(Constants.OpeningBracket, i);
                if (indexOfNextOpening == -1) break;
                i = indexOfNextOpening;
                int realIndex = i - removedCount;

                bool res = ParseModifier(
                    lineCopy, i, out ParsedDialogueTrigger parsedDialogueTrigger,
                    out bool isClosing, out int count);

                bool incrementIndex = false;

                if (!isClosing)
                {
                    parsedDialogueTrigger.StartingIndex = realIndex;
                    stagedTriggers.Add(parsedDialogueTrigger);
                    incrementIndex = true;
                }
                else
                {
                    int matchIndex = FindIndexWithCommandText(parsedDialogueTrigger.CommandText, stagedTriggers);
                    if (matchIndex == -1)
                    {
                        Debug.LogErrorFormat("mismatched-matched command text </{0}> ({1})",
                            parsedDialogueTrigger.CommandText, rawLine);
                    }

                    // Move from staged to list once we know the length.
                    var opener = stagedTriggers[matchIndex];
                    opener.Length = i - opener.StartingIndex - removedCount - inserted;
                    if (IndexIsSpace(realIndex - 1, line.Text))
                    {
                        opener.Length -= 1;
                    }
                    parsedTriggers.Add(opener);

                    stagedTriggers.RemoveAt(matchIndex);
                }

                // Trim string
                // if (realIndex > 0 && line.Text[realIndex - 1] == ' ' &&
                //     realIndex + count < line.Text.Length && line.Text[realIndex + count] == ' ')
                // {
                //     count++;
                // }
                // Debug.LogFormat("Removing substring ({0})", line.Text.Substring(realIndex, count));
                Debug.LogFormat("Removed to ({0})", line.Text);
                line.Text = line.Text.Remove(realIndex, count);

                // replace with ' ' if necessary
                if (realIndex != 0 && realIndex != line.Text.Length - 1 && !IndexIsSpace(realIndex - 1, line.Text) && !IndexIsSpace(realIndex, line.Text))
                {
                    line.Text = line.Text.Insert(realIndex, " ");
                    count--;
                    // i++;
                    // inserted++;

                    if (incrementIndex)
                    {
                        var temp = stagedTriggers[stagedTriggers.Count - 1];
                        temp.StartingIndex += 1;
                        stagedTriggers[stagedTriggers.Count - 1] = temp;
                    }
                }

                removedCount += count;
                i += count - 1;
            }

            for (int i = 0; i < stagedTriggers.Count; i++)
            {
                parsedTriggers.Add(stagedTriggers[i]);
            }

            Debug.LogFormat("Removed to ({0})", line.Text);
            line.Text = RemoveLeadingTrailingWhiteSpace(line.Text);
            triggers = parsedTriggers.ToArray();
        }

        private static int FindIndexWithCommandText(string text, List<ParsedDialogueTrigger> staged)
        {
            for (int i = 0; i < staged.Count; i++)
            {
                if (text == staged[i].CommandText)
                    return i;
            }

            return -1;
        }

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

            int startingIndex = index + (isOpening ? 1 : 2);
            length = endingIndex - index + 1;
            modifier = line.Substring(startingIndex, endingIndex - startingIndex);

            return true;
        }

        private static bool ParseModifier(
            string line, int index, out ParsedDialogueTrigger trigger, out bool isClosing, out int countToRemove)
        {
            trigger = new ParsedDialogueTrigger();
            isClosing = false;
            countToRemove = -1;
            if (index < 0 || index >= line.Length)
            {
                Debug.LogErrorFormat("invalid index {0} for line {1}", index, line);
                return false;
            }

            char c = line[index];
            if (c != Constants.OpeningBracket)
            {
                Debug.LogErrorFormat(
                    "Internal Error: letter at index {0} not opening bracket ({1})", c, Constants.OpeningBracket);
                return false;
            }

            int endingIndex = line.IndexOf(Constants.ClosingBracket, index + 1);
            if (endingIndex == -1)
            {
                Debug.LogErrorFormat("found opening bracket, but could not find closing bracket ({0})", line);
                return false;
            }

            if (index + 1 == endingIndex)
            {
                Debug.LogErrorFormat("bracket is empty ({0})", line);
                return false;
            }

            // original text: <shake 2> asdfsa
            // modifierText = "shake 2"
            countToRemove = endingIndex - index + 1;

            // exclude opening and closing bracket
            string modifierText = line.Substring(index + 1, countToRemove - 2);
            // string[] texts = RemoveLeadingTrailingWhiteSpace(modifierText).Split(' ');
            string[] texts = modifierText.Split(' ');
            // texts = RemoveLeadingTrailingWhiteSpace(texts);
            int processedCount = 0;

            // Check if the first character is '/'
            string firstText = texts[processedCount];
            if (firstText[0] == Constants.ClosingIdentifier)
            {
                isClosing = true;

                // Proceed to next word if 
                if (firstText.Length == 1)
                {
                    processedCount++;
                }
            }

            // Check if the first text ends with ','
            if (IsActorID(texts[processedCount], out trigger.ActorID))
            {
                processedCount++;
            }

            if (texts.Length - processedCount <= 0)
            {
                Debug.LogErrorFormat("DialogueTrigger ({0}) doesn't contain a commandText");
                return false;
            }

            // Extract
            trigger.CommandText = texts[processedCount];

            //special case if command is first string and concat-ed with '/' (like /shake -> shake)
            if (isClosing && processedCount == 0)
            {
                trigger.CommandText = trigger.CommandText.Substring(1, trigger.CommandText.Length - 1);
            }

            processedCount++;

            // Check if there are remaining args
            if (texts.Length - processedCount <= 0)
            {
                trigger.Args = null;
                return true;
            }

            // Copy remaning strings as args
            trigger.Args = new string[texts.Length - processedCount];
            System.Array.Copy(texts, processedCount, trigger.Args, 0, trigger.Args.Length);

            return true;
        }

        private static bool IsActorID(string text, out string ID)
        {
            ID = "";
            int lastCharIndex = text.Length - 1;
            if (text[lastCharIndex] != Constants.ActorIdentifier)
            {
                return false;
            }

            ID = text.Remove(lastCharIndex);
            return true;
        }

        // /// <summary>
        // /// Removes trailing and leading white space from a string.
        // /// </summary>
        // /// <param name="text">Text to trim.</param>
        // /// <returns>Trimmed version of the string.</returns>
        // private static string[] RemoveLeadingTrailingWhiteSpace(string[] texts)
        // {
        //     int count = 0;
        //     for (int i = 0; i < texts.Length; i++)
        //     {
        //         texts[i] = RemoveLeadingTrailingWhiteSpace(texts[i]);
        //         if (texts[i] != "")
        //             count++;
        //     }

        //     // HACKY way to remove entires in the array that is empty because split
        //     string[] res = new string[count];
        //     int j = 0;
        //     for (int i = 0; i < texts.Length; i++)
        //     {
        //         if (texts[i] != "")
        //         {
        //             res[j] = texts[i];
        //             j++;
        //         }
        //     }
        //     return res;
        // }

    }
}