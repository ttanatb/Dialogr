using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilr;

namespace Dialogr
{
    public class DialogueUnityEvent : UnityEvent<ParsedDialogue, UnityAction>
    {
    }

    public class DialogueManager : Singleton<DialogueManager>
    {
        // TODO: add background dialogue UIs

        [SerializeField]
        protected Yarn.Unity.DialogueUI m_mainDialogueUI = null;
        protected UnityAction<string> m_onLineUpdateAction = null;
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

        protected DialogueUnityEvent m_onDialogueUpdate = new DialogueUnityEvent();
        public DialogueUnityEvent OnDialogueUpdate
        {
            get { return m_onDialogueUpdate; }
        }

        protected UnityEvent m_onDialogueStart = new UnityEvent();
        public UnityEvent OnDialogueStart
        {
            get { return m_onDialogueStart; }
        }

        protected UnityEvent m_onDialogueEnd = new UnityEvent();
        public UnityEvent OnDialogueEnd
        {
            get { return m_onDialogueEnd; }
        }

        protected void RegisterListeners(Yarn.Unity.DialogueUI dialogueUI)
        {
            m_onLineUpdateAction = (line) => { OnLineUpdate(line, dialogueUI); };
            dialogueUI.onLineUpdate.AddListener(m_onLineUpdateAction);
            dialogueUI.onDialogueStart.AddListener(OnDialogueStarted);
            dialogueUI.onDialogueEnd.AddListener(OnDialogueEnded);
        }

        protected void RemoveListeners(Yarn.Unity.DialogueUI dialogueUI)
        {
            dialogueUI.onLineUpdate.RemoveListener(m_onLineUpdateAction);
            dialogueUI.onDialogueStart.RemoveListener(OnDialogueStarted);
            dialogueUI.onDialogueEnd.RemoveListener(OnDialogueEnded);
        }

        protected void Start()
        {
            // Run all the on set code
            RegisterListeners(m_mainDialogueUI);
        }

        protected void OnDestroy()
        {
            if (m_mainDialogueUI != null)
                RemoveListeners(m_mainDialogueUI);
        }

        protected void OnDialogueStarted()
        {
            m_onDialogueStart.Invoke();
        }

        protected void OnDialogueEnded()
        {
            m_onDialogueEnd.Invoke();
        }

        protected void OnLineUpdate(string line, Yarn.Unity.DialogueUI dialogueUI)
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
