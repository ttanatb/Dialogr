using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Dialogue
{
    public class DialogueUnityEvent : UnityEvent<ParsedDialogue, UnityAction>
    {
    }

    public class DialogueManager : Singleton<DialogueManager>
    {
        // TODO: add background dialogue UIs

        [SerializeField]
        private Yarn.Unity.DialogueUI m_mainDialogueUI = null;
        public Yarn.Unity.DialogueUI MainDialogueUI
        {
            get
            {
                return m_mainDialogueUI;
            }
            set
            {
                if (m_mainDialogueUI != null)
                {
                    m_mainDialogueUI.onLineUpdate.RemoveListener(OnLineUpdate);
                    m_mainDialogueUI.onDialogueStart.RemoveListener(OnDialogueStarted);
                    m_mainDialogueUI.onDialogueEnd.RemoveListener(OnDialogueEnded);
                }

                m_mainDialogueUI = value;
                if (m_mainDialogueUI == null) return;

                m_mainDialogueUI.onLineUpdate.AddListener(OnLineUpdate);
                m_mainDialogueUI.onDialogueStart.AddListener(OnDialogueStarted);
                m_mainDialogueUI.onDialogueEnd.AddListener(OnDialogueStarted);
            }
        }

        private DialogueUnityEvent m_onDialogueUpdate = new DialogueUnityEvent();
        public DialogueUnityEvent OnDialogueUpdate
        {
            get { return m_onDialogueUpdate; }
        }

        private UnityEvent m_onDialogueStart = new UnityEvent();
        public UnityEvent OnDialogueStart
        {
            get { return m_onDialogueStart; }
        }

        private UnityEvent m_onDialogueEnd = new UnityEvent();
        public UnityEvent OnDialogueEnd
        {
            get { return m_onDialogueEnd; }
        }

        private void Start()
        {
            // Run all the on set code
            MainDialogueUI = m_mainDialogueUI;
        }

        void OnDialogueStarted()
        {
            m_onDialogueStart.Invoke();
        }

        void OnDialogueEnded()
        {
            m_onDialogueEnd.Invoke();
        }

        void OnLineUpdate(string line)
        {
            // TODO: make this work background dialogue UI
            ParsedDialogue parsedDialogue = Parser.Parse(line);
            m_onDialogueUpdate.Invoke(parsedDialogue,
            /*onLineCompletedCb=*/ () =>
             {
                 m_mainDialogueUI.MarkLineComplete();
             });
        }
    }
}
