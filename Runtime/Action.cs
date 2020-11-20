using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PBAS
{
    [Serializable, CreateAssetMenu (fileName = "New Action", menuName = "PBAS/Action")]
    public class Action : ScriptableObject
    {
        [Serializable]
        public class Parameter
        {
            [SerializeField]
            private string m_name;
            [SerializeField]
            private List<Property> m_requiredPropertyList = new List<Property> ();
            [SerializeField]
            private List<Property> m_forbiddenPropertyList = new List<Property> ();

            public string Name { get => m_name; }
            public List<Property> RequiredPropertyList { get => m_requiredPropertyList; }
            public List<Property> ForbiddenPropertyList { get => m_forbiddenPropertyList; }

            public List<string> RequiredPropertyNames { get => GetPropertyNames (ref m_requiredPropertyList); }
            public List<string> ForbiddenPropertyNames { get => GetPropertyNames (ref m_forbiddenPropertyList); }

            public bool IsAnyRequiredProperty { get => IsAnyPropertyInList (ref m_requiredPropertyList); }
            public bool IsAnyForbiddenProperty { get => IsAnyPropertyInList (ref m_forbiddenPropertyList); }

            private List<string> GetPropertyNames (ref List<Property> propertyList)
            {
                var names = new List<string> ();

                foreach (Property property in propertyList)
                {
                    names.Add (property.name);
                }

                return names;
            }

            private bool IsAnyPropertyInList (ref List<Property> propertyList)
            {
                return propertyList.Any ();
            }
        }

        public enum EPreconditionComparison
        {
            Equal, NotEqual
        }

        [Serializable]
        public class Precondition
        {
            [SerializeField]
            private EPreconditionComparison m_comparison;
            [SerializeField]
            private int m_leftParameterIndex;
            [SerializeField]
            private int m_leftPropertyIndex;
            [SerializeField]
            private int m_leftFieldIndex;
            [SerializeField]
            private int m_rightParameterIndex;
            [SerializeField]
            private int m_rightPropertyIndex;
            [SerializeField]
            private int m_rightFieldIndex;
            [SerializeField]
            private PropertyFieldValue m_rightValue;

            public EPreconditionComparison Comparison { get => m_comparison; }
            public int LeftParameterIndex { get => m_leftParameterIndex; }
            public int LeftPropertyIndex { get => m_leftPropertyIndex; }
            public int LeftFieldteIndex { get => m_leftFieldIndex; }
            public int RightParameterIndex { get => m_rightParameterIndex; }
            public int RightPropertyIndex { get => m_rightPropertyIndex; }
            public int RightFieldteIndex { get => m_rightFieldIndex; }
            public PropertyFieldValue RightValue { get => m_rightValue; }
        }

        public enum EEffectOperation
        {
            SetValue, AddProperty, RemoveProperty
        }

        [Serializable]
        public class Effect
        {
            [SerializeField]
            private EEffectOperation m_operation;
            [SerializeField]
            private int m_setParameterIndex;
            [SerializeField]
            private int m_setPropertyIndex;
            [SerializeField]
            private int m_setFieldIndex;
            [SerializeField]
            private PropertyFieldValue m_setFieldValue;
            [SerializeField]
            private Property m_targetProperty;

            public EEffectOperation Operation { get => m_operation; }
            public int SetParameterIndex { get => m_setParameterIndex; }
            public int SetPropertyIndex { get => m_setPropertyIndex; }
            public int SetFieldIndex { get => m_setFieldIndex; }
            public PropertyFieldValue SetFieldValue { get => m_setFieldValue; }
            public Property TargetProperty { get => m_targetProperty; }
        }

        [SerializeField]
        private List<Parameter> m_parameterList = new List<Parameter> ();
        [SerializeField]
        private List<Precondition> m_preconditionList = new List<Precondition> ();
        [SerializeField]
        private List<Effect> m_effectList = new List<Effect> ();

        public List<Parameter> ParameterList { get => m_parameterList; }
        public bool IsAnyParameter { get => ParameterList.Any (); }

        public List<Precondition> PreconditionList { get => m_preconditionList; }
        public bool IsAnyPrecondition { get => PreconditionList.Any (); }

        public List<Effect> EffectList { get => m_effectList; }
        public bool IsAnyEffect { get => EffectList.Any (); }

        public List<string> ParameterNames
        {
            get
            {
                var names = new List<string> ();

                foreach (var parameter in ParameterList)
                {
                    names.Add (parameter.Name);
                }

                return names;
            }
        }
    }
}
