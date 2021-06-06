using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharTween;
using DG.Tweening;

public class UITextShaker : UITextBehaviour
{
    protected override void Start()
    {
        base.Start();
        m_commandID = "shake";
        RegisterTextModifier();
    }

    protected override void TriggerBehaviour(
        List<Tweener> modifiers,
        CharTweener charTweener,
        int startingIndex,
        int count,
        string[] args)
    {
        if (args.Length > 0)
        {
            Debug.LogWarningFormat("Shake command issued with unsupported args");
        }

        for (int i = startingIndex; i < startingIndex + count; i++)
        {
            modifiers.Add(charTweener.DOShakePosition(
                    i, 1, 1, 50, 90, false, false)
                .SetLoops(-1, LoopType.Restart));
        }
    }
}
