using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using UnityEngine.UI;
using Dialogr;

namespace Tests
{
    public class UINameViewTest
    {
        TextMeshProUGUI text = null;
        UINameView view = null;
        Image img = null;
        Image childImg = null;

#if UNITY_EDITOR
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            CreateUINameViewObj(out  text, out  view, out  img, out  childImg);
            yield return new EnterPlayMode();
        }
#endif

        private void CreateUINameViewObj(out TextMeshProUGUI text, out UINameView name, out Image img, out Image childImg)
        {
            GameObject gameObject = new GameObject(
                "Test Obj", typeof(UINameView), typeof(Image));
            GameObject textObj = new GameObject("Text", typeof(TextMeshProUGUI));
            textObj.transform.SetParent(gameObject.transform);
            GameObject child = new GameObject("Test Child", typeof(Image));
            child.transform.SetParent(gameObject.transform);

            gameObject.TryGetComponent(out name);
            gameObject.TryGetComponent(out img);
            textObj.TryGetComponent(out text);
            child.TryGetComponent(out childImg);
        }


        [UnityTest]
        public IEnumerator SetName()
        {
            view.SetName("Test");
            Assert.AreEqual("Test", text.text);
            yield return null;
        }


        [UnityTest]
        public IEnumerator SetVisible()
        {

            view.SetVisible(true);
            Assert.IsTrue(img.enabled);
            Assert.IsTrue(childImg.enabled);

            view.SetName("Test");

            view.SetVisible(false);
            Assert.AreEqual("", text.text);
            Assert.IsFalse(img.enabled);
            Assert.IsFalse(childImg.enabled);
            yield return null;
        }
    }
}
