using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Dialogr;

namespace Tests
{
    public class DialogueParserTest
    {
        [Test]
        public void Parse()
        {
            ParsedDialogue result = Parser.Parse("name: generic dialogue");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("name", result.ActorID);
            Assert.AreEqual("generic dialogue", result.Line);

            result = Parser.Parse("another extended line.");
            Assert.AreEqual("another extended line.", result.Line);
        }

        [Test]
        public void TrimTrailingLeadingWhiteSpace()
        {
            ParsedDialogue result = Parser.Parse(" name :    \t dialogue   ");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("name", result.ActorID);
            Assert.AreEqual("dialogue", result.Line);
        }

        [Test]
        public void ParseAttribute()
        {
            ParsedDialogue result = Parser.Parse("name: generic <shake>shakey</shake> dialogue");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("name", result.ActorID);
            Assert.AreEqual("generic shakey dialogue", result.Line);

            Assert.IsNotNull(result.Attributes);
            Assert.AreEqual(1, result.Attributes.Length);

            TextAttribute attrib = result.Attributes[0];
            Assert.AreEqual(TextAttribute.Type.Shake, attrib.ModType);
            Assert.AreEqual(8, attrib.StartingIndex);
            Assert.AreEqual(6, attrib.Length);
        }
    }
}
