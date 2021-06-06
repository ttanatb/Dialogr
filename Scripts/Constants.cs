using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogr
{
    public class Constants
    {
        public static readonly HashSet<char> Punctuation = new HashSet<char> { ',', '.', '?', '!' };
        public static readonly HashSet<char> Space = new HashSet<char> { ' ', '\t' };
        public static readonly HashSet<string> FunctionsWithNoObjects =
            new HashSet<string> { "shake" };

        // TODO define this base off a config instead.
        // public static readonly Dictionary<string, TextModifier.Type> AttribDef =
        //     new Dictionary<string, TextModifier.Type>
        //     {
        //     { "shake", TextModifier.Type.Shake },
        //     };
        public static readonly char OpeningBracket = '<';
        public static readonly char ClosingBracket = '>';
        public static readonly char ClosingIdentifier = '/';
        public static readonly char ActorIdentifier = ',';
    }
}