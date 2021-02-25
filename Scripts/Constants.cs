using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class Constants
    {
        public static readonly HashSet<char> Punctuation = new HashSet<char> { ',', '.', '?', '!' };
        public static readonly HashSet<char> Space = new HashSet<char> { ' ', '\t' };
        public static readonly HashSet<string> FunctionsWithNoObjects =
            new HashSet<string> { "shake" };

        // TODO define this base off a config instead.
        public static readonly Dictionary<string, TextAttribute.Type> AttribDef =
            new Dictionary<string, TextAttribute.Type>
            {
            { "shake", TextAttribute.Type.Shake },
            };
        public static readonly char OpeningBracket = '<';
        public static readonly char ClosingBracket = '>';
    }
}