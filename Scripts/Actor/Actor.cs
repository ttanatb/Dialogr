using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogr
{
    public class Actor : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        protected Animator m_animator = null;

        [SerializeField]
        protected string m_isTalkingAnimParm = "";

        [SerializeField]
        protected string[] m_aliases = null;

        [SerializeField]
        protected Vector3 m_anchorOffset = Vector3.zero;

        protected ActorManager m_actorManager = null;
        protected Navigator m_navigator = null;
        #endregion // Fields

        #region Properties
        public string[] Aliases
        {
            get { return m_aliases; }
            set { m_aliases = value; }
        }
        #endregion // Properties

        #region Public, virtual methods
        public virtual void SetTalking(bool isTalking)
        {
            m_animator.SetBool(m_isTalkingAnimParm, isTalking);
        }

        public virtual Vector3 GetDialogueAnchor()
        {
            return transform.position + m_anchorOffset;
        }

        public virtual void MoveTo(ITargetable target)
        {
            if (m_navigator == null) return;

            m_navigator.MoveTo(target);
        }
        #endregion // Public, virtual methods

        #region Unity Messages
        protected virtual void Start()
        {
            // Should characters self-add like this? who knows
            m_actorManager = ActorManager.Instance;
            m_actorManager.AddActor(m_aliases, this);
        }

        private void OnDestroy()
        {
            m_actorManager.RemoveActor(m_aliases);
        }
        #endregion // Unity Messages
    }
}