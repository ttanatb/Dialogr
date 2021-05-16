using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogr
{
    public class ActorSprite : Actor
    {
        const float kDialogueBubbleMargin = 0.5f;

        private SpriteRenderer[] m_sprites = null;
        private Vector3 m_dialogueAnchor = Vector3.zero;
        private SpriteRenderer[] Sprites
        {
            get
            {
                if (m_sprites == null || m_sprites.Length == 0)
                {
                    m_sprites = GetComponentsInChildren<SpriteRenderer>();
                }
                return m_sprites;
            }
        }

        private void RecalculateBounds()
        {
            var sprites = Sprites;
            Vector3 min = Vector3.one * float.PositiveInfinity;
            Vector3 max = Vector3.one * float.NegativeInfinity;

            foreach (var s in sprites)
            {
                min.x = Mathf.Min(s.bounds.min.x, min.x);
                min.y = Mathf.Min(s.bounds.min.y, min.y);

                max.x = Mathf.Max(s.bounds.max.x, max.x);
                max.y = Mathf.Max(s.bounds.max.y, max.y);
            }

            var center = (max - min) / 2.0f + min;
            center.z = transform.position.z;

            m_dialogueAnchor = new Vector3
            {
                x = 0,
                y = max.y - min.y + kDialogueBubbleMargin,
                z = 0,
            };

            m_dialogueAnchor += m_anchorOffset;
        }

        public override Vector3 GetDialogueAnchor()
        {
            RecalculateBounds();
            return transform.position + m_dialogueAnchor;
        }

        protected override void Start()
        {
            // Make sure to cache the bound calculation
            RecalculateBounds();
            base.Start();
        }
    }
}