using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilr;

namespace Dialogr
{
    [System.Serializable]
    public struct ActorCommand
    {
        public string ActorID;
        public string CommandText;
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

        protected HashSet<string> m_registeredTextModifiers = new HashSet<string>();

        protected Dictionary<ActorCommand, System.Action<string[]>> m_registeredDialogueTriggers = new Dictionary<ActorCommand, System.Action<string[]>>();

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

        public void RegisterTextModifiers(string[] commandIDs)
        {
            foreach (var id in commandIDs)
                m_registeredTextModifiers.Add(id);
        }

        public void RegisterTextModifier(string commandID)
        {
            m_registeredTextModifiers.Add(commandID);
        }

        public void RemoveTextModifiers(string[] commandIDs)
        {
            foreach (var id in commandIDs)
                m_registeredTextModifiers.Remove(id);
        }

        public void RemoveTextModifier(string commandID)
        {
            m_registeredTextModifiers.Remove(commandID);
        }

        public void RegisterDialogueTrigger(ActorCommand key, System.Action<string[]> value)
        {
            m_registeredDialogueTriggers.Add(key, value);
        }

        public void RemoveDialogueTriggers(ActorCommand[] actorCommands)
        {
            foreach (var cmd in actorCommands)
                m_registeredDialogueTriggers.Remove(cmd);
        }

        public void RemoveDialogueTrigger(ActorCommand actorCommand)
        {
            m_registeredDialogueTriggers.Remove(actorCommand);
        }

        protected void OnLineUpdate(string line, Yarn.Unity.DialogueUI dialogueUI)
        {
            ParsedDialogue parsedDialogue = Parser.Parse(line);

            // parse to modifiers/triggers
            TextModifier[] modifiers = new TextModifier[parsedDialogue.Triggers.Length];
            DialogueTrigger[] triggers = new DialogueTrigger[parsedDialogue.Triggers.Length];
            int j = 0;
            int k = 0;
            for (int i = 0; i < modifiers.Length; i++)
            {
                var t = parsedDialogue.Triggers[i];
                var actorCmdPair = new ActorCommand()
                {
                    ActorID = t.ActorID,
                    CommandText = t.CommandText,
                };
                // t.CommandText is a DialogueTrigger
                if (m_registeredDialogueTriggers.ContainsKey(actorCmdPair))
                {
                    triggers[k] = new DialogueTrigger()
                    {
                        StartingIndex = t.StartingIndex,
                        Args = t.Args,
                        Callback = m_registeredDialogueTriggers[actorCmdPair],
                    };
                    k++;
                }
                else if (m_registeredTextModifiers.Contains(t.CommandText))
                {
                    modifiers[j] = new TextModifier()
                    {
                        CommandText = t.CommandText,
                        Args = t.Args,
                        StartingIndex = t.StartingIndex,
                        Length = t.Length,
                    };
                    j++;
                }
                else
                {
                    Debug.LogErrorFormat("Unrecognized dialogue trigger: {0}", t.CommandText);
                }
            }

            DialogueModel model = new DialogueModel()
            {
                ActorID = parsedDialogue.ActorID,
                DisplayName = parsedDialogue.DisplayName,
                DialogueType = DialogueType.Speech,
                Text = parsedDialogue.Line.Text,
                Modifiers = modifiers,
                Triggers = triggers,
                // TODO:
                // TalkingSFX = 
            };

            Yarn.Unity.DialogueUI dialogue = dialogueUI;
            m_onDialogueUpdate.Invoke(model,
            /*onLineCompletedCb=*/ () =>
             {
                 dialogue.MarkLineComplete();
             });
        }
    }
}
