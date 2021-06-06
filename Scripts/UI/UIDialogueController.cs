using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using Dialogr;
using UnityEngine.InputSystem;

namespace UI
{
    public class UIDialogueController : MonoBehaviour
    {
        UINameView m_uiNameView = null;
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

            m_uiNameView = GetComponentInChildren<UINameView>();
            m_uiDialogueView = GetComponentInChildren<UIDialogueView>();
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
            m_completeDialogueAction.action.Enable();
            DisplayUI(true);
        }

        void OnDialogueUpdate(DialogueModel model, UnityAction dialogueCompletedCb)
        {
            m_completedText = false;
            // TODO: filter between main dialogue and background dialogue

            // Set name if it was provided.
            if (model.DisplayName != "")
                m_uiNameView.SetName(model.DisplayName);

            if (model.ActorID != "")
            {
                Actor actor = m_actorManager.GetActor(model.ActorID);
                if (actor != null)
                {
                    m_uiDialogueView.SetActor(actor);
                }
                else
                {
                    Debug.LogErrorFormat("Unable to find actor with ID: {0}", model.ActorID);
                }
            }

            m_uiDialogueView.Clear();
            m_uiDialogueView.DisplayText(model, /*onLineDisplayedCb=*/ () =>
            {
                m_completedText = true;
                m_uiDialogueView.ShowBlinker(true);
            });

            m_dialogueCompletedCb = dialogueCompletedCb;
        }

        void OnDialogueEnd()
        {
            m_completeDialogueAction.action.Disable();
            DisplayUI(false);
        }

        void DisplayUI(bool shouldShow)
        {
            m_uiNameView.SetVisible(shouldShow);
            m_uiDialogueView.SetVisible(shouldShow);
        }
    }
}
