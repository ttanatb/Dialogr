using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text;

namespace Dialogr
{
    public struct TextAttribute
    {
        public enum Type
        {
            None,
            Shake,
        }

        public Type ModType;
        public int StartingIndex;
        public int Length;
    }

    public enum DialogueType
    {
        Speech,
        Thought,
    };

    public struct ParsedDialogue
    {
        public string ActorID;
        public string DisplayName;
        public DialogueType DialogueType;
        public string Line;
        public TextAttribute[] Attributes;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(
                "ParsedDialogue:\nActorID: {0}\nDisplayName: {1}\nDialogueType: {2}\nLine: {3}\nAttribs:\n",
                ActorID, DisplayName, DialogueType, Line
            );
            if (Attributes != null)
            {
                for (int i = 0; i < Attributes.Length; i++)
                {
                    sb.AppendFormat("\t{0}: {1}", i, Attributes[i]);
                }
            }
            return sb.ToString();
        }
    }


    public struct DialogueCallbacks
    {
        public UnityAction Callback;
        public string Object;
        public string Function;
        public string[] Args;
        public int Index;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{Object:{0}, Function:{1}, Args:[", Object, Function);
            foreach (var item in Args)
            {
                sb.AppendFormat("{0}, ", item);
            }
            sb.AppendFormat("], Index:{0}", Index);
            return sb.ToString();
        }
    }
}