using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text;

namespace Dialogr
{
    // <shake arg1> shake-y text </shake>
    public struct TextModifier
    {
        public string CommandText;
        public string[] Args;
        public int StartingIndex;
        public int Length;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ID: {0}, Args:[", CommandText);
            if (Args != null)
            {
                foreach (var item in Args)
                {
                    sb.AppendFormat("{0}, ", item);
                }
            }
            sb.AppendFormat("], StartingIndex: {0}, Length: {1}", StartingIndex, Length);
            return sb.ToString();
        }
    }

    public struct DialogueTrigger
    {
        public int StartingIndex;
        public string[] Args;
        public System.Action<string[]> Callback;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{StartingIndex: {0}, Args:[", StartingIndex);
            foreach (var item in Args)
            {
                sb.AppendFormat("{0}, ", item);
            }
            return sb.ToString();
        }
    }

    public enum DialogueType
    {
        Speech,
        Thought,
    };

    public struct DialogueModel
    {
        // ID of the actor who's talking
        public string ActorID;
        // The name to display
        public string DisplayName;
        public DialogueType DialogueType;
        public string Text;
        public Audior.AudioClipInfo TalkingSFX;
        public TextModifier[] Modifiers;
        public DialogueTrigger[] Triggers;
    }

    public class DialogueUnityEvent : UnityEvent<DialogueModel, UnityAction>
    {
    }

    // bob: random text <anim smile> more text
    //      ID         = anim
    //      ObjectName = bob
    //      Args       = [smile]
    //
    // surprising gossip! <bob, anim gasp> like what
    //      ID         = anim
    //      ObjectName = bob
    //      Args       = [gasp]
    public struct ParsedDialogueTrigger
    {
        public string CommandText;
        public string ActorID;
        public int StartingIndex;
        public int Length;
        public string[] Args;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{ID: {0}, ObjectName {1} Args:[", CommandText, ActorID);
            foreach (var item in Args)
            {
                sb.AppendFormat("{0}, ", item);
            }
            sb.AppendFormat("], StartingIndex: {0}", StartingIndex);
            return sb.ToString();
        }
    }

    public struct ParsedLine
    {
        public DialogueType DialogueType;
        public string Text;
    }

    public struct ParsedDialogue
    {
        // ID of the actor who's talking
        public string ActorID;

        // The name to display
        public string DisplayName;

        // Dialogue triggers
        public ParsedDialogueTrigger[] Triggers;

        // The actual line and associated metadata
        public ParsedLine Line;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ParsedDialogue:");
            sb.AppendFormat("ActorID: {0} DisplayName: {1}\n", ActorID, DisplayName);
            if (Triggers != null)
            {
                sb.AppendLine("DialogueTriggers: [");
                for (int i = 0; i < Triggers.Length; i++)
                {
                    sb.AppendFormat("\t{0}: {1}", i, Triggers[i]);
                }
                sb.AppendLine("],");
            }
            sb.AppendFormat("DialogueModel: {0}", Line);
            return sb.ToString();
        }
    }

}