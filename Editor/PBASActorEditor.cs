using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PBAS
{
    [CustomEditor (typeof (PBASActor))]
    public class PBASActorEditor : Editor
    {
        private int m_pickerID = -1;
        private Dictionary<string, bool> m_fieldsFoldoutDict = new Dictionary<string, bool> ();

        public override void OnInspectorGUI ()
        {
            var actor = target as PBASActor;
            var removeProperties = new List<Property> ();

            if (Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID () == m_pickerID)
            {
                Object pickedObject = EditorGUIUtility.GetObjectPickerObject ();

                if (pickedObject)
                {
                    var picked = pickedObject as Property;
                    actor.AddProperty (picked);
                }

                m_pickerID = -1;
            }

            foreach (var propertyPair in actor.PropertyDataDict)
            {
                string propertyName = propertyPair.Key;
                PBASActor.PropertyData propertyData = propertyPair.Value;

                if (!propertyData.Property)
                {
                    removeProperties.Add (propertyData.Property);
                    continue;
                }

                MyEditorLayout.Horizontal (() =>
                {
                    GUILayout.Label (propertyName, GUILayout.ExpandWidth (false));

                    if (GUILayout.Button ("X", GUILayout.ExpandWidth (false)))
                    {
                        removeProperties.Add (propertyData.Property);
                    }
                });

                bool bFold;

                if (m_fieldsFoldoutDict.TryGetValue (propertyName, out bFold) == false)
                {
                    bFold = false;
                    m_fieldsFoldoutDict.Add (propertyName, false);
                }

                MyEditorLayout.FoldoutHeaderGroup ("Fields", ref bFold, () =>
                {
                    foreach (var field in propertyData.Property.FieldList)
                    {
                        string fieldName = field.Name;
                        PropertyFieldValue fieldValue = propertyData.GetFieldValue (fieldName);

                        GUILayout.Label ($"(Type: {fieldValue.Type.ToString ()}) {fieldName} = {fieldValue.ToString ()}", GUILayout.ExpandWidth (false));
                    }
                });

                m_fieldsFoldoutDict[propertyName] = bFold;
                MyEditorLayout.HorizontalLine ();
            }

            foreach (Property remove in removeProperties)
            {
                if (remove)
                {
                    actor.RemoveProperty (remove);
                }
            }

            if (GUILayout.Button ("Add Property"))
            {
                m_pickerID = GUIUtility.GetControlID (FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<Property> (null, false, string.Empty, m_pickerID);
            }

            serializedObject.ApplyModifiedProperties ();
        }
    }
}
