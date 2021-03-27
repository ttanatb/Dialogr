using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilr;

namespace Dialogue
{
    public class ActorManager : Singleton<ActorManager>
    {
        // Lookup table of ID to character
        private Dictionary<int, Actor> m_IDtoActor = null;

        // Lookup table of alias -> ID
        private Dictionary<string, int> m_aliasToID = null;

        private void Awake()
        {
            m_IDtoActor = new Dictionary<int, Actor>();
            m_aliasToID = new Dictionary<string, int>();
        }

        public void AddActor(IEnumerable<string> aliases, Actor actor)
        {
            int newID = m_IDtoActor.Count;
            if (m_IDtoActor.ContainsValue(actor))
            {
                // Character already exists >:(
                Debug.LogErrorFormat("Character {0} already exists", actor.gameObject.name);
                return;
            }

            if (aliases == null)
            {
                Debug.LogErrorFormat("Aliases unexpectedly null for character {0}", actor.name);
            }

            foreach (var item in aliases)
            {
                m_aliasToID.Add(item, newID);
            }
            m_IDtoActor.Add(newID, actor);
        }

        public Actor GetActor(string alias)
        {
            Actor a = null;
            if (!m_aliasToID.ContainsKey(alias))
                return a;

            int id = m_aliasToID[alias];
            if (!m_IDtoActor.ContainsKey(id))
            {
                Debug.LogErrorFormat("Unexpected unable to find character for alias {0} and id {1}", alias, id);
                return a;
            }

            a = m_IDtoActor[id];
            return a;
        }

        public void RemoveActor(IEnumerable<string> aliases) 
        {
            foreach (var alias in aliases)
            {
                var id = m_aliasToID[alias];
                m_IDtoActor.Remove(id);
                m_aliasToID.Remove(alias);
            }
        }
    }
}
