using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Dialogr
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Navigator : MonoBehaviour
    {

        private NavMeshAgent m_navMeshAgent = null;

        // Start is called before the first frame update
        void Start()
        {
            TryGetComponent(out m_navMeshAgent);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void MoveTo(ITargetable target)
        {
            bool res = m_navMeshAgent.SetDestination(target.GetLocation());
            if (!res)
            {
                Debug.LogWarningFormat("{0} is unable to navigate to {1}", gameObject.name, target.ToString());
            }
        }
    }
}
