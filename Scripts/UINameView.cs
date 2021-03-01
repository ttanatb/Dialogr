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
        m_nameText = GetComponentInChildren<TextMeshProUGUI>();
        if (m_nameText == null)
        {
            Debug.LogErrorFormat("{0} does not contain TextMeshProUGUI component", gameObject);
        }
    }

    public override void SetVisible(bool shouldShow)
    {
        if (!shouldShow) m_nameText.text = "";
        base.SetVisible(shouldShow);
    }
}
