using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;
using Dialogue;
using UnityEngine.TestTools.Utils;

namespace Tests
{
    public class DialogueManagerTest
    {
        private DialogueManager m_dialogueManager = null;
        private Yarn.Unity.DialogueRunner m_dialogueRunner = null;
        private Yarn.Unity.DialogueUI m_dialogueUI = null;
        private UIDialogueView m_view = null;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            SceneManager.LoadScene("DialogueUIScene");
            bool loaded = false;
            SceneManager.sceneLoaded += (index, mode) =>
            {
                loaded = true;
            };
            yield return new WaitUntil(() => loaded);

            m_dialogueManager = DialogueManager.Instance;
            m_dialogueRunner = GameObject.FindObjectOfType<Yarn.Unity.DialogueRunner>();
            m_dialogueUI = GameObject.FindObjectOfType<Yarn.Unity.DialogueUI>();
            m_view = GameObject.FindObjectOfType<UIDialogueView>();

            m_dialogueManager.MainDialogueUI = m_dialogueUI;
        }

        [UnityTest]
        public IEnumerator EventsFired()
        {
            bool isStartCbCalled = false;
            UnityAction onDialogueStartCb = () => { isStartCbCalled = true; };
            m_dialogueManager.OnDialogueStart.AddListener(onDialogueStartCb);

            bool isEndCbCalled = false;
            UnityAction onDialogueEndCb = () => { isEndCbCalled = true; };
            m_dialogueManager.OnDialogueEnd.AddListener(onDialogueEndCb);

            ParsedDialogue res = new ParsedDialogue();
            UnityAction onLineCompletedcb = null;
            bool isLineUpdateCbCalled = false;
            UnityAction<ParsedDialogue, UnityAction> lineUpdateCb = (dialogue, cb) =>
            {
                res = dialogue;
                onLineCompletedcb = cb;
                isLineUpdateCbCalled = true;
            };
            m_dialogueManager.OnDialogueUpdate.AddListener(lineUpdateCb);

            m_dialogueRunner.StartDialogue("TwoLines");
            yield return new WaitUntil(() => isStartCbCalled);

            Assert.IsTrue(isStartCbCalled);
            Assert.IsTrue(isLineUpdateCbCalled);
            Assert.AreEqual("line 1", res.Line);

            isLineUpdateCbCalled = false;
            onLineCompletedcb.Invoke();
            yield return new WaitUntil(() => isLineUpdateCbCalled);

            Assert.IsTrue(isLineUpdateCbCalled);
            Assert.AreEqual("line 2", res.Line);

            onLineCompletedcb.Invoke();
            yield return new WaitUntil(() => isEndCbCalled);

            Assert.IsTrue(isEndCbCalled);

            m_dialogueManager.OnDialogueStart.RemoveListener(onDialogueStartCb);
            m_dialogueManager.OnDialogueEnd.RemoveListener(onDialogueEndCb);
            m_dialogueManager.OnDialogueUpdate.RemoveListener(lineUpdateCb);
        }
    }
}
