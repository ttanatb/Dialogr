using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;
using Dialogr;
using UnityEngine.TestTools.Utils;

namespace Tests
{
    public class ActorSpriteTest
    {
        private ActorSprite m_actorSprite = null;
        private Animator m_animator = null;

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

            m_actorSprite = GameObject.FindObjectOfType<ActorSprite>();
            m_actorSprite.TryGetComponent(out m_animator);
        }

        [UnityTest]
        public IEnumerator SetTalking()
        {
            m_actorSprite.SetTalking(true);
            Assert.IsTrue(m_animator.GetBool("testBool"));

            m_actorSprite.SetTalking(false);
            Assert.IsFalse(m_animator.GetBool("testBool"));
            yield return null;
        }

        [UnityTest]
        public IEnumerator GetDialogueAnchor()
        {
            var pos = m_actorSprite.GetDialogueAnchor();
            var expected = new Vector3(0.15f, 0.9f, 0.0f);

            Assert.That(pos, Is.EqualTo(expected).Using(Vector3EqualityComparer.Instance));
            yield return null;
        }
    }
}