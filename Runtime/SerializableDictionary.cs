using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBAS
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> m_keys = new List<TKey> ();
        [SerializeField]
        private List<TValue> m_values = new List<TValue> ();

        public void OnAfterDeserialize ()
        {
            Clear ();

            if (m_keys.Count != m_values.Count)
            {
                throw new Exception ("Key count and value count are not same!");
            }

            for (int i = 0; i < m_keys.Count; i++)
            {
                Add (m_keys[i], m_values[i]);
            }
        }

        public void OnBeforeSerialize ()
        {
            m_keys.Clear ();
            m_values.Clear ();

            foreach (var pair in this)
            {
                m_keys.Add (pair.Key);
                m_values.Add (pair.Value);
            }
        }
    }
}
