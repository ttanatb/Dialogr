using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class Actor : MonoBehaviour
    {
        [SerializeField]
        protected Animator m_animator = null;

        [SerializeField]
        protected string m_isTalkingAnimParm = "";

        [SerializeField]
        protected string[] m_aliases = null;

        public string[] Aliases {
            get {return m_aliases;}
            set { m_aliases = value;}
        }

        [SerializeField]
        protected Vector3 m_anchorOffset = Vector3.zero;

        protected ActorManager m_actorManager = null;

        public virtual void SetTalking(bool isTalking)
        {
            m_animator.SetBool(m_isTalkingAnimParm, isTalking);
        }

        protected virtual void Start()
        {
            // Should characters self-add like this? who knows
            m_actorManager = ActorManager.Instance;
            m_actorManager.AddActor(m_aliases, this);
        }

        public virtual Vector3 GetDialogueAnchor()
        {
            return transform.position + m_anchorOffset;
        }

        private void OnDestroy() {
            m_actorManager.RemoveActor(m_aliases);
        }
    }
}