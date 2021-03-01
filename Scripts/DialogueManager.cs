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
        UnityAction<string> m_onLineUpdateAction = null;
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
                    RemoveListeners(m_mainDialogueUI);
                }

                m_mainDialogueUI = value;
                if (m_mainDialogueUI == null) return;

                RegisterListeners(m_mainDialogueUI);
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

        private void RegisterListeners(Yarn.Unity.DialogueUI dialogueUI)
        {
            m_onLineUpdateAction = (line) => { OnLineUpdate(line, dialogueUI); };
            dialogueUI.onLineUpdate.AddListener(m_onLineUpdateAction);
            dialogueUI.onDialogueStart.AddListener(OnDialogueStarted);
            dialogueUI.onDialogueEnd.AddListener(OnDialogueEnded);
        }

        private void RemoveListeners(Yarn.Unity.DialogueUI dialogueUI)
        {
            dialogueUI.onLineUpdate.RemoveListener(m_onLineUpdateAction);
            dialogueUI.onDialogueStart.RemoveListener(OnDialogueStarted);
            dialogueUI.onDialogueEnd.RemoveListener(OnDialogueEnded);
        }

        private void Start()
        {
            // Run all the on set code
            RegisterListeners(m_mainDialogueUI);
        }

        private void OnDestroy()
        {
            if (m_mainDialogueUI != null)
                RemoveListeners(m_mainDialogueUI);
        }

        void OnDialogueStarted()
        {
            m_onDialogueStart.Invoke();
        }

        void OnDialogueEnded()
        {
            m_onDialogueEnd.Invoke();
        }

        void OnLineUpdate(string line, Yarn.Unity.DialogueUI dialogueUI)
        {
            ParsedDialogue parsedDialogue = Parser.Parse(line);
            Yarn.Unity.DialogueUI dialogue = dialogueUI;
            m_onDialogueUpdate.Invoke(parsedDialogue,
            /*onLineCompletedCb=*/ () =>
             {
                 dialogue.MarkLineComplete();
             });
        }
    }
}
