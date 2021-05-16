using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIView : MonoBehaviour
{
    protected UnityEngine.UI.Image[] m_renderers = null;

    protected UnityEngine.UI.Image[] Renderers
    {
        get
        {
            if (m_renderers == null || m_renderers.Length == 0)
            {
                m_renderers = GetComponentsInChildren<UnityEngine.UI.Image>();
            }
            return m_renderers;
        }
    }

    public virtual void SetVisible(bool shouldShow)
    {
        foreach (var r in Renderers)
        {
            r.enabled = shouldShow;
        }
    }
}
