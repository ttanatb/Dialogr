using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UINameView : MonoBehaviour
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
}
