using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{

    public class Utils
    {
        private static RectTransform m_canvas = null;

        public static RectTransform CanvasTransform
        {
            get
            {
                if (m_canvas == null)
                {
                    Canvas c = (Canvas)Object.FindObjectOfType(typeof(Canvas));
                    c.TryGetComponent(out m_canvas);
                }
                return m_canvas;
            }
        }

        public static Vector3 ConvertWorldPosToCanvasPos(Vector3 worldPos)
        {
            return ConvertWorldPosToCanvasPos(worldPos, Vector3.zero);
        }

        public static Vector3 ConvertWorldPosToCanvasPos(Vector3 worldPos, Vector3 offset)
        {
            var screenPos = Camera.main.WorldToViewportPoint(worldPos);

            screenPos.x *= CanvasTransform.rect.size.x;
            screenPos.y *= CanvasTransform.rect.size.y;

            return screenPos + offset;
        }
    }

}