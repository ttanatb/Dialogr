using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class Actor : MonoBehaviour
    {
        [SerializeField]
        private Animator m_animator = null;

        [SerializeField]
        private string m_isTalkingAnimParm = "";

        [SerializeField]
        private string[] m_aliases = null;

        public void SetTalking(bool isTalking)
        {
            m_animator.SetBool(m_isTalkingAnimParm, isTalking);
        }

        private void Start()
        {
            // Should characters self-add like this? who knows
            // CharacterManager.Instance.AddCharacter(m_aliases, this);
        }
    }
}