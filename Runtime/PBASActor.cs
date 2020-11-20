using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBAS
{
    public class PBASActor : MonoBehaviour
    {
        [Serializable]
        public class PropertyFieldValueDictionary : SerializableDictionary<string, PropertyFieldValue>
        {
        }

        [Serializable]
        public class PropertyData
        {
            [SerializeField]
            private Property m_property;
            [SerializeField]
            private PropertyFieldValueDictionary m_fieldValueDictionary = new PropertyFieldValueDictionary ();

            public Property Property { get => m_property; set => m_property = value; }
            public PropertyFieldValueDictionary FieldValueDictionary { get => m_fieldValueDictionary; }

            public PropertyFieldValue GetFieldValue (string name)
            {
                PropertyFieldValue value;

                if (m_fieldValueDictionary.TryGetValue (name, out value) == false)
                {
                    PropertyField found = m_property.FieldList.Find ((field) => field.Name == name);
                    value = found.DefaultValue;
                }

                return value;
            }

            public void SetFieldValue (string name, PropertyFieldValue value)
            {
                if (m_fieldValueDictionary.ContainsKey (name))
                {
                    m_fieldValueDictionary[name] = value;
                }
                else
                {
                    m_fieldValueDictionary.Add (name, value);
                }
            }
        }

        [Serializable]
        public class PropertyDataDictionary : SerializableDictionary<string, PropertyData>
        {
        }

        [SerializeField]
        private PropertyDataDictionary m_propertyData;

        private PBASAnimator m_animator;

        public PropertyDataDictionary PropertyDataDict { get => m_propertyData; }

        private void Awake ()
        {
            m_animator = GetComponent<PBASAnimator> ();
        }

        public void AddProperty (Property property)
        {
            if (m_propertyData.ContainsKey (property.name))
            {
                return;
            }

            var data = new PropertyData ();
            data.Property = property;

            m_propertyData.Add (property.name, data);
        }

        public void RemoveProperty (Property property)
        {
            if (m_propertyData.ContainsKey (property.name))
            {
                m_propertyData.Remove (property.name);
            }
        }

        public bool CheckProperty (List<Property> checkList)
        {
            foreach (Property property in checkList)
            {
                if (m_propertyData.ContainsKey (property.name) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public List<PBASActor> FindTargetWithProperty (List<Property> requiredProperties, List<Property> forbiddenProperties)
        {
            List<PBASActor> found = new List<PBASActor> ();

            foreach (var target in FindObjectsOfType<PBASActor> ())
            {
                if (target.CheckProperty (requiredProperties) == false)
                {
                    continue;
                }

                if (target.CheckProperty (forbiddenProperties))
                {
                    continue;
                }

                found.Add (target);
            }

            return found;
        }

        public bool FindPossibleActionParameters (Action action, out Dictionary<string, List<PBASActor>> possibleParameters)
        {
            bool bAllParameters = true;
            possibleParameters = new Dictionary<string, List<PBASActor>> ();

            foreach (var parameter in action.ParameterList)
            {
                List<PBASActor> foundTargets = FindTargetWithProperty (parameter.RequiredPropertyList, parameter.ForbiddenPropertyList);

                if (foundTargets.Count <= 0)
                {
                    bAllParameters = false;
                    break;
                }

                foundTargets.Sort ((first, second) =>
                {
                    float toFirst = (first.transform.position - transform.position).sqrMagnitude;
                    float toNext = (second.transform.position - transform.position).sqrMagnitude;

                    if (toFirst > toNext)
                    {
                        return 1;
                    }
                    else if (toNext > toFirst)
                    {
                        return -1;
                    }

                    return 0;
                });

                possibleParameters.Add (parameter.Name, foundTargets);
            }

            return bAllParameters;
        }

        public Dictionary<string, PBASActor> GetActionParameters (Action action, Dictionary<string, List<PBASActor>> possibleParameters)
        {
            var parameters = new Dictionary<string, PBASActor> ();

            foreach (var possiblePair in possibleParameters)
            {
                parameters.Add (possiblePair.Key, possiblePair.Value[0]);
            }

            return parameters;
        }

        public bool CheckPrecondition (Action action, Dictionary<string, PBASActor> parameters)
        {
            foreach (Action.Precondition precondition in action.PreconditionList)
            {
                Action.Parameter lParam = action.ParameterList[precondition.LeftParameterIndex];
                Property lProperty = lParam.RequiredPropertyList[precondition.LeftPropertyIndex];
                PropertyField lField = lProperty.FieldList[precondition.LeftFieldteIndex];
                PropertyFieldValue lValue = parameters[lParam.Name].m_propertyData[lProperty.name].GetFieldValue (lField.Name);
                PropertyFieldValue rValue;

                if (precondition.RightParameterIndex < 0)
                {
                    rValue = precondition.RightValue;
                }
                else
                {
                    Action.Parameter rParam = action.ParameterList[precondition.RightParameterIndex];
                    Property rProperty = rParam.RequiredPropertyList[precondition.RightPropertyIndex];
                    PropertyField rField = rProperty.FieldList[precondition.RightFieldteIndex];
                    rValue = parameters[rParam.Name].m_propertyData[rProperty.name].GetFieldValue (rField.Name);
                }

                bool bSucceed = false;

                switch (precondition.Comparison)
                {
                    case Action.EPreconditionComparison.Equal:
                        bSucceed = lValue == rValue;
                        break;
                    case Action.EPreconditionComparison.NotEqual:
                        bSucceed = lValue != rValue;
                        break;
                }

                if (bSucceed == false)
                {
                    break;
                }
            }

            return true;
        }

        public void ExecuteEffect (Action action, Dictionary<string, PBASActor> parameters)
        {
            foreach (Action.Effect effect in action.EffectList)
            {
                switch (effect.Operation)
                {
                    case Action.EEffectOperation.SetValue:
                    {
                        Action.Parameter parameter = action.ParameterList[effect.SetParameterIndex];
                        Property property = parameter.RequiredPropertyList[effect.SetPropertyIndex];
                        PropertyField field = property.FieldList[effect.SetFieldIndex];
                        parameters[parameter.Name].m_propertyData[property.name].SetFieldValue (field.Name, effect.SetFieldValue);

                        break;
                    }

                    case Action.EEffectOperation.AddProperty:
                    {
                        Action.Parameter parameter = action.ParameterList[effect.SetParameterIndex];
                        parameters[parameter.Name].AddProperty (effect.TargetProperty);
                        break;
                    }

                    case Action.EEffectOperation.RemoveProperty:
                    {
                        Action.Parameter parameter = action.ParameterList[effect.SetParameterIndex];
                        Property property = parameter.RequiredPropertyList[effect.SetPropertyIndex];
                        parameters[parameter.Name].RemoveProperty (property);
                        break;
                    }
                }
            }
        }

        public bool ExecuteAction (Action action)
        {
            Dictionary<string, List<PBASActor>> possibleParams;

            if (FindPossibleActionParameters (action, out possibleParams) == false)
            {
                return false;
            }

            Dictionary<string, PBASActor> parameters = GetActionParameters (action, possibleParams);

            if (CheckPrecondition (action, parameters) == false)
            {
                return false;
            }

            // After action is finished
            ExecuteEffect (action, parameters);

            if (m_animator)
            {
                m_animator.PlayActionAnimation (action);
            }

            return true;
        }
    }
}
