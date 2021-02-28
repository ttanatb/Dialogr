using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UINameView : UIView
{
    TextMeshProUGUI m_nameText = null;

    public void SetName(string name)
    {
        m_nameText.text = name;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_nameText == null)
            TryGetComponent(out m_nameText);
    }

    public override void SetVisible(bool shouldShow)
    {
        if (!shouldShow) m_nameText.text = "";
        base.SetVisible(shouldShow);
    }
}
