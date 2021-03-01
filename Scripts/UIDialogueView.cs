using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CharTween;
using DG.Tweening;
using System.Text;
using UnityEngine.Events;
using Dialogue;

public class UIDialogueView : UIView
{
    [SerializeField]
    Animator m_animator = null;
    [SerializeField]
    string m_animParamShowTicker = "";

    const float kFadeDuration = 0.05f;
    const float kLocalMoveYFromPos = 3.0f;
    const float kPunctuationPauseSec = 0.2f;
    const float kDefaultTextInterval = 0.01f;

    TextMeshProUGUI m_text = null;
    CharTweener m_charTweener = null;
    Sequence m_displaySequence = null;
    UnityAction m_onLineDisplayedCb = null;
    List<Tweener> m_modifiers = null;
    Actor m_talkingActor = null;
    List<DialogueCallbacks> m_callbacks = null;
    RectTransform m_rectTransform = null;

    public void Clear()
    {
        m_text.text = "";
        ClearModifiers();
        ClearCallbacks();
    }

    public void SetActor(Actor actor)
    {
        m_talkingActor = actor;
        MoveTo(actor);
    }

    protected virtual void MoveTo(Actor actor)
    {
        m_rectTransform.anchoredPosition =
            UI.Utils.ConvertWorldPosToCanvasPos(actor.GetDialogueAnchor());
    }

    public override void SetVisible(bool shouldShow)
    {
        if (!shouldShow) m_text.text = "";
        base.SetVisible(shouldShow);
    }

    public void DisplayText(string text, UnityAction onLineDisplayedCb = null)
    {
        m_text.text = text;
        m_onLineDisplayedCb = onLineDisplayedCb;
        if (text == "")
            return;

        m_displaySequence = DOTween.Sequence();
        bool prevIsSpace = true;

        for (int charIndex = 0; charIndex < text.Length; charIndex++)
        {
            char c = text[charIndex];
            bool isSpace = Constants.Space.Contains(c);
            float totalDelay = kDefaultTextInterval;

            // Add extra pause for punctuation
            if (Constants.Punctuation.Contains(c))
                totalDelay += kPunctuationPauseSec;

            // TODO: maybe add variation in color?
            m_charTweener.SetColor(charIndex, new Color(0, 0, 0, 0));
            m_displaySequence
                .Join(m_charTweener.DOColor(charIndex, Color.black, kFadeDuration).SetDelay(totalDelay));

            // Append callbacks like anim triggers or screen shake
            if (IndexInCallbackList(charIndex, out UnityAction action))
                m_displaySequence.AppendCallback(() => { action.Invoke(); });

            // Play talking SFX if talking. Start/stop talking anim as necessary.
            if (isSpace)
            {
                if (!prevIsSpace && m_talkingActor != null)
                    m_displaySequence.AppendCallback(() => { m_talkingActor.SetTalking(false); });
            }
            else
            {
                // Play talking SFX.
                // if (model.TalkingSFX.AudioClip != null)
                // m_displaySequence.AppendCallback(() => { m_audioManager.PlayOneShot(model.TalkingSFX); });

                if (prevIsSpace && m_talkingActor != null)
                    m_displaySequence.AppendCallback(() => { m_talkingActor.SetTalking(true); });
            }

            prevIsSpace = isSpace;
        }

        m_displaySequence.AppendCallback(OnDisplayLineComplete).Play();
    }

    public void SkipDisplayAnim()
    {
        if (!m_displaySequence.active)
            return;

        m_displaySequence.Complete();
        OnDisplayLineComplete();
    }

    public void ClearCallbacks()
    {
        m_callbacks = null;
    }

    public void SetCallbacks(List<DialogueCallbacks> callbacks)
    {
        m_callbacks = callbacks;
    }

    public void ClearModifiers()
    {
        foreach (var tweener in m_modifiers)
        {
            tweener.fullPosition = 0;
            tweener.Kill();
        }
        m_modifiers.Clear();
    }

    public void SetTextModifier(IEnumerable<TextAttribute> attributes)
    {
        foreach (var attrib in attributes)
        {
            switch (attrib.ModType)
            {
                case TextAttribute.Type.Shake:
                    ShakeTextAt(attrib.StartingIndex, attrib.Length);
                    break;
                    // TODO: add more text modifiers
            }
        }
    }

    public void ShowBlinker(bool shouldShow)
    {
        m_animator.SetBool(m_animParamShowTicker, shouldShow);
    }

    private void ShakeTextAt(int startingIndex, int count)
    {
        for (int i = startingIndex; i < startingIndex + count; i++)
        {
            m_modifiers.Add(
                m_charTweener.DOShakePosition(i, 1, 1, 50, 90, false, false)
                .SetLoops(-1, LoopType.Restart));
        }
    }

    private void OnDisplayLineComplete()
    {
        m_displaySequence.Kill(false);
        m_onLineDisplayedCb?.Invoke();

        // Stop all talking anims
        if (m_talkingActor != null)
        {
            m_talkingActor.SetTalking(false);
        }
    }

    private bool IndexInCallbackList(int index, out UnityAction action)
    {
        action = null;
        if (m_callbacks == null)
            return false;

        foreach (var cb in m_callbacks)
        {
            if (cb.Index == index)
            {
                action = cb.Callback;
                return true;
            }
        }

        return false;
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_text = GetComponentInChildren<TextMeshProUGUI>();
        m_charTweener = m_text.GetCharTweener();
        m_modifiers = new List<Tweener>();
    }

    private void OnDestroy()
    {
        m_onLineDisplayedCb = null;
    }

    void Start()
    {
        TryGetComponent(out m_rectTransform);
    }
}
