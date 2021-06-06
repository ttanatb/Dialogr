using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharTween;
using DG.Tweening;

[RequireComponent(typeof(UIDialogueView))]
public abstract class UITextBehaviour : MonoBehaviour
{
    [SerializeField]
    protected string m_commandID = "";
    protected UIDialogueView m_dialogueView = null;

    protected virtual void Start()
    {
        TryGetComponent(out m_dialogueView);

        if (m_commandID == "")
        {
            Debug.LogErrorFormat("UITextBehavior({0}) has invalid command ID: {1}", this, m_commandID);
        }

        m_dialogueView.RegisterBehaviour(m_commandID, TriggerBehaviour);
    }

    protected void RegisterTextModifier()
    {
        Dialogr.DialogueManager.Instance.RegisterTextModifier(m_commandID);
    }

    protected abstract void TriggerBehaviour(
        List<Tweener> modifiers,
        CharTweener charTweener,
        int startingIndex,
        int count,
        string[] args);
}
