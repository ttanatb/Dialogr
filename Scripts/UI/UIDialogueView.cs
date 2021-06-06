using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CharTween;
using DG.Tweening;
using System.Text;
using UnityEngine.Events;
using Dialogr;
using Audior;

using TextBehaviourAction = System.Action<System.Collections.Generic.List<DG.Tweening.Tweener>, CharTween.CharTweener, int, int, string[]>;
using DialogueAction = System.Action<string[]>;

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
    DialogueTrigger[] m_triggers = null;
    RectTransform m_rectTransform = null;
    Dictionary<string, TextBehaviourAction> m_registeredModifiers = null;

    AudioManager m_audioManager = null;

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

    public void DisplayText(DialogueModel model, UnityAction onLineDisplayedCb = null)
    {
        m_text.text = model.Text;
        m_onLineDisplayedCb = onLineDisplayedCb;
        if (model.Text == "")
            return;

        m_triggers = model.Triggers;
        m_displaySequence = DOTween.Sequence();
        bool prevIsSpace = true;

        for (int charIndex = 0; charIndex < model.Text.Length; charIndex++)
        {
            char c = model.Text[charIndex];
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
            if (IndexInCallbackList(charIndex, out DialogueTrigger trigger))
                m_displaySequence.AppendCallback(() => { trigger.Callback.Invoke(trigger.Args); });

            // Play talking SFX if talking. Start/stop talking anim as necessary.
            if (isSpace)
            {
                if (!prevIsSpace && m_talkingActor != null)
                    m_displaySequence.AppendCallback(() => { m_talkingActor.SetTalking(false); });
            }
            else
            {
                // Play talking SFX.
                if (model.TalkingSFX.AudioClip != null)
                    m_displaySequence.AppendCallback(() => { m_audioManager.PlayOneShot(model.TalkingSFX); });

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
        m_triggers = null;
    }

    public void SetCallbacks(DialogueTrigger[] triggers)
    {
        m_triggers = triggers;
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

    public void ApplyTextModifiers(IEnumerable<TextModifier> attributes)
    {
        foreach (var attrib in attributes)
        {
            if (!m_registeredModifiers.ContainsKey(attrib.CommandText))
            {
                Debug.LogErrorFormat("Encountered unhandled text modifier: {0}", attrib.CommandText);
                continue;
            }

            m_registeredModifiers[attrib.CommandText].Invoke(
                m_modifiers, m_charTweener, attrib.StartingIndex, attrib.Length, attrib.Args);
        }
    }

    public void ShowBlinker(bool shouldShow)
    {
        m_animator.SetBool(m_animParamShowTicker, shouldShow);
    }

    public void RegisterBehaviour(string command, TextBehaviourAction callback)
    {
        m_registeredModifiers.Add(command, callback);
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

    private bool IndexInCallbackList(int index, out DialogueTrigger trigger)
    {
        trigger = new DialogueTrigger();
        if (m_triggers == null)
            return false;

        foreach (var t in m_triggers)
        {
            if (t.StartingIndex == index)
            {
                trigger = t;
                return true;
            }
        }

        return false;
    }

    void Awake()
    {
        m_text = GetComponentInChildren<TextMeshProUGUI>();
        m_charTweener = m_text.GetCharTweener();

        m_modifiers = new List<Tweener>();
        m_registeredModifiers = new Dictionary<string, TextBehaviourAction>();
    }

    private void OnDestroy()
    {
        m_onLineDisplayedCb = null;
    }

    void Start()
    {
        TryGetComponent(out m_rectTransform);

        m_audioManager = AudioManager.Instance;
    }
}
