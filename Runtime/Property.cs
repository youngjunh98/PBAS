using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PBAS
{
    [Serializable, CreateAssetMenu (fileName = "New Property", menuName = "PBAS/Property")]
    public class Property : ScriptableObject
    {
        [SerializeField]
        private List<PropertyField> m_fieldList = new List<PropertyField> ();

        public List<PropertyField> FieldList { get => m_fieldList; }
        public bool IsAnyField { get => FieldList.Any (); }

        public List<string> FieldNames
        {
            get
            {
                var names = new List<string> ();

                foreach (var field in FieldList)
                {
                    names.Add (field.Name);
                }

                return names;
            }
        }
    }
}
