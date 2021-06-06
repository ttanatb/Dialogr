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
        public void ParseLine()
        {
            ParsedDialogue result = Parser.Parse("name: generic dialogue");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("name", result.ActorID);
            Assert.AreEqual("generic dialogue", result.Line.Text);

            result = Parser.Parse("another extended line.");
            Assert.AreEqual("another extended line.", result.Line.Text);
        }

        [Test]
        public void ParseActorIDAndName()
        {
            ParsedDialogue result = Parser.Parse("name <bob>: dialogue");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("bob", result.ActorID);
            Assert.AreEqual("dialogue", result.Line.Text);

            result = Parser.Parse("<dan> missy: woweeee");
            Assert.AreEqual("missy", result.DisplayName);
            Assert.AreEqual("dan", result.ActorID);
            Assert.AreEqual("woweeee", result.Line.Text);
        }

        [Test]
        public void ParseDialogueTrigger_Simple()
        {
            ParsedDialogue result = Parser.Parse("name: <smile> dialogue");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("name", result.ActorID);
            Assert.AreEqual("dialogue", result.Line.Text);

            Assert.IsNotNull(result.Triggers);
            Assert.AreEqual(1, result.Triggers.Length);

            ParsedDialogueTrigger trigger = result.Triggers[0];
            Assert.AreEqual("smile", trigger.CommandText);
            Assert.AreEqual("", trigger.ActorID);
            Assert.AreEqual(0, trigger.StartingIndex);
            Assert.AreEqual(0, trigger.Length);
            Assert.IsNull(trigger.Args);
        }

        [Test]
        public void ParseDialogueTrigger_Complex()
        {
            ParsedDialogue result = Parser.Parse("name: <bob, smile arg0 arg1> dialogue");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("name", result.ActorID);
            Assert.AreEqual("dialogue", result.Line.Text);

            Assert.IsNotNull(result.Triggers);
            Assert.AreEqual(1, result.Triggers.Length);

            ParsedDialogueTrigger trigger = result.Triggers[0];
            Assert.AreEqual("smile", trigger.CommandText);
            Assert.AreEqual("bob", trigger.ActorID);
            Assert.AreEqual(0, trigger.StartingIndex);
            Assert.AreEqual(0, trigger.Length);
            Assert.IsNotNull(trigger.Args);

            Assert.AreEqual(2, trigger.Args.Length);
            Assert.AreEqual("arg0", trigger.Args[0]);
            Assert.AreEqual("arg1", trigger.Args[1]);
        }

        [Test]
        public void ParseDialogueTrigger_Multiple()
        {
            ParsedDialogue result = Parser.Parse("name: <bob, smile arg0 arg1><shake> dialogue </shake>");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("name", result.ActorID);
            Assert.AreEqual("dialogue", result.Line.Text);

            Assert.IsNotNull(result.Triggers);
            Assert.AreEqual(2, result.Triggers.Length);

            ParsedDialogueTrigger trigger = result.Triggers[1];
            Assert.AreEqual("smile", trigger.CommandText);
            Assert.AreEqual("bob", trigger.ActorID);
            Assert.AreEqual(0, trigger.StartingIndex);
            Assert.AreEqual(0, trigger.Length);
            Assert.IsNotNull(trigger.Args);

            Assert.AreEqual(2, trigger.Args.Length);
            Assert.AreEqual("arg0", trigger.Args[0]);
            Assert.AreEqual("arg1", trigger.Args[1]);

            trigger = result.Triggers[0];
            Assert.AreEqual("shake", trigger.CommandText);
            Assert.AreEqual("", trigger.ActorID);
            Assert.AreEqual(0, trigger.StartingIndex);
            Assert.AreEqual(8, trigger.Length);
            Assert.IsNull(trigger.Args);
        }

        // TODO: <bake><shake> potato </bake><shake>
        // this will still work but do i care?

        [Test]
        public void TrimWhiteSpace_ExtraLeadingTrailing()
        {
            ParsedDialogue result = Parser.Parse(" name<id> :  <shake> text </ shake> ");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("id", result.ActorID);
            Assert.AreEqual("text", result.Line.Text);

            Assert.IsNotNull(result.Triggers);
            Assert.AreEqual(1, result.Triggers.Length);

            ParsedDialogueTrigger trigger = result.Triggers[0];
            Assert.AreEqual("shake", trigger.CommandText);
        }

        [Test]
        public void TrimWhiteSpace_SpaceSurroundingTrigger()
        {
            ParsedDialogue result = Parser.Parse("name: the <shake> brown <bake> cat </bake> </shake> sits");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("the brown cat sits", result.Line.Text);

            ParsedDialogue result2 = Parser.Parse("name: the <shake> brown <bake> cat </bake></shake> sits");
            Assert.AreEqual("name", result2.DisplayName);
            Assert.AreEqual("the brown cat sits", result2.Line.Text);
        }

        [Test]
        public void TrimWhiteSpace_SpaceInTriggers()
        {
            ParsedDialogue result = Parser.Parse(" name < id   > :  <  bob,  shake   strong   2   > text < / shake  > ");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("id", result.ActorID);
            Assert.AreEqual("text", result.Line.Text);

            Assert.IsNotNull(result.Triggers);
            Assert.AreEqual(1, result.Triggers.Length);

            ParsedDialogueTrigger trigger = result.Triggers[0];
            Assert.AreEqual("shake", trigger.CommandText);
            Assert.AreEqual("bob", trigger.ActorID);
            Assert.AreEqual(0, trigger.StartingIndex);
            Assert.AreEqual(4, trigger.Length);

            Assert.IsNotNull(trigger.Args);
            Assert.AreEqual(2, trigger.Args.Length);
            Assert.AreEqual("strong", trigger.Args[0]);
            Assert.AreEqual("2", trigger.Args[1]);
        }

        [Test]
        public void ParseAttribute()
        {
            ParsedDialogue result = Parser.Parse("name: generic <shake>shakey</shake> dialogue");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("name", result.ActorID);
            Assert.AreEqual("generic shakey dialogue", result.Line.Text);

            Assert.IsNotNull(result.Triggers);
            Assert.AreEqual(1, result.Triggers.Length);

            ParsedDialogueTrigger trigger = result.Triggers[0];
            Assert.AreEqual("shake", trigger.CommandText);
            Assert.IsNull(trigger.Args);
            Assert.AreEqual(8, trigger.StartingIndex);
            Assert.AreEqual(6, trigger.Length);
        }

        [Test]
        public void ParseAttributeMultiple()
        {
            ParsedDialogue result = Parser.Parse("name: generic <shake><bake>shakey bakey</bake></shake> dialogue");
            Assert.AreEqual("name", result.DisplayName);
            Assert.AreEqual("name", result.ActorID);
            Assert.AreEqual("generic shakey bakey dialogue", result.Line.Text);

            Assert.IsNotNull(result.Triggers);
            Assert.AreEqual(2, result.Triggers.Length);

            ParsedDialogueTrigger trigger = result.Triggers[0];
            Assert.AreEqual("bake", trigger.CommandText);
            Assert.IsNull(trigger.Args);
            Assert.AreEqual(8, trigger.StartingIndex);
            Assert.AreEqual(12, trigger.Length);

            trigger = result.Triggers[1];
            Assert.AreEqual("shake", trigger.CommandText);
            Assert.IsNull(trigger.Args);
            Assert.AreEqual(8, trigger.StartingIndex);
            Assert.AreEqual(12, trigger.Length);
        }
    }
}
