using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using UnityEngine.InputSystem;

namespace UI
{
    public class UIDialogueController : MonoBehaviour
    {
        [SerializeField]
        UINameView m_uiNameView = null;
        [SerializeField]
        UIDialogueView m_uiDialogueView = null;
        [SerializeField]
        InputActionReference m_completeDialogueAction = null;

        ActorManager m_actorManager = null;
        DialogueManager m_dialogueManager = null;
        bool m_completedText = false;
        UnityAction m_dialogueCompletedCb = null;

        private void TriggerNextDialogue(InputAction.CallbackContext ctx)
        {
            m_uiDialogueView.ShowBlinker(false);
            if (m_completedText)
                m_dialogueCompletedCb.Invoke();
            else
                m_uiDialogueView.SkipDisplayAnim();
        }

        // Start is called before the first frame update
        void Start()
        {
            m_dialogueManager = DialogueManager.Instance;
            m_actorManager = ActorManager.Instance;

            m_dialogueManager.OnDialogueStart.AddListener(OnDialogueStart);
            m_dialogueManager.OnDialogueUpdate.AddListener(OnDialogueUpdate);
            m_dialogueManager.OnDialogueEnd.AddListener(OnDialogueEnd);

            m_completeDialogueAction.action.performed += TriggerNextDialogue;
        }

        private void OnDestroy()
        {
            m_dialogueManager.OnDialogueStart.RemoveListener(OnDialogueStart);
            m_dialogueManager.OnDialogueUpdate.RemoveListener(OnDialogueUpdate);
            m_dialogueManager.OnDialogueEnd.RemoveListener(OnDialogueEnd);

            m_completeDialogueAction.action.performed -= TriggerNextDialogue;
        }

        void OnDialogueStart()
        {
            DisplayUI(true);
        }

        void OnDialogueUpdate(ParsedDialogue dialogue, UnityAction dialogueCompletedCb)
        {
            // TODO: filter between main dialogue and background dialogue

            // Set name if it was provided.
            if (dialogue.DisplayName != "")
                m_uiNameView.SetName(dialogue.DisplayName);

            Actor actor = m_actorManager.GetActor(dialogue.ActorID);
            if (actor != null)
            {
                m_uiDialogueView.SetActor(actor);
            }

            m_uiDialogueView.Clear();
            m_uiDialogueView.SetTextModifier(dialogue.Attributes);
            m_uiDialogueView.DisplayText(dialogue.Line, /*onLineDisplayedCb=*/ () =>
            {
                m_completedText = true;
                m_uiDialogueView.ShowBlinker(true);
            });

            m_dialogueCompletedCb = dialogueCompletedCb;
        }

        void OnDialogueEnd()
        {
            DisplayUI(false);
        }

        void DisplayUI(bool shouldShow)
        {

        }
    }
}
