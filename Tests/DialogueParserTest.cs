using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
// using Dialogue;

// namespace Tests
// {
//     public class DialogueParserTest
//     {
//         [Test]
//         public void DialogueParser_Parse()
//         {
//             TestBasic();
//             TestTrailingLeadingWhiteSpace();
//             TestAttribute();
//         }

//         private void TestBasic()
//         {
//             ParsedDialogue result = DialogueParser.Parse("name: generic dialogue");
//             Assert.AreEqual("name", result.DisplayName);
//             Assert.AreEqual("name", result.ActorID);
//             Assert.AreEqual("generic dialogue", result.Line);

//             result = DialogueParser.Parse("another extended line.");
//             Assert.AreEqual("another extended line.", result.Line);
//         }

//         private void TestTrailingLeadingWhiteSpace()
//         {
//             ParsedDialogue result = DialogueParser.Parse(" name :    \t dialogue   ");
//             Assert.AreEqual("name", result.DisplayName);
//             Assert.AreEqual("name", result.ActorID);
//             Assert.AreEqual("dialogue", result.Line);
//         }

//         private void TestAttribute()
//         {
//             ParsedDialogue result = DialogueParser.Parse("name: generic <shake>shakey</shake> dialogue");
//             Assert.AreEqual("name", result.DisplayName);
//             Assert.AreEqual("name", result.ActorID);
//             Assert.AreEqual("generic shakey dialogue", result.Line);

//             Assert.IsNotNull(result.Attributes);
//             Assert.AreEqual(1, result.Attributes.Length);

//             TextAttribute attrib = result.Attributes[0];
//             Assert.AreEqual(TextAttribute.Type.Shake, attrib.ModType);
//             Assert.AreEqual(8, attrib.StartingIndex);
//             Assert.AreEqual(6, attrib.Length);
//         }
//     }
// }
