using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PBAS
{
    [CustomEditor (typeof (Property))]
    public class PropertyEditor : Editor
    {
        private bool m_bShowFields = true;
        public override void OnInspectorGUI ()
        {
            var property = target as Property;

            var serializedFieldList = serializedObject.FindProperty ("m_fieldList");
            m_bShowFields = EditorGUILayout.Foldout (m_bShowFields, "Fields");

            if (m_bShowFields)
            {
                MyEditorUtility.ForEachSerializedArray (serializedFieldList, (serializedField, fieldIndex) =>
                {
                    var serializedName = serializedField.FindPropertyRelative ("m_name");
                    EditorGUILayout.PropertyField (serializedName);

                    MyEditorLayout.Horizontal (() =>
                    {
                        GUILayout.Label ("Default Value");

                        var serializedDefaultValue = serializedField.FindPropertyRelative ("m_defaultValue");
                        DrawPropertyFieldInput (serializedDefaultValue);
                    });

                    EditorGUILayout.Separator ();
                });
            }

            MyEditorLayout.Horizontal (() =>
            {
                if (GUILayout.Button ("Add"))
                {
                    property.FieldList.Add (new PropertyField ());
                    serializedObject.Update ();
                }

                if (GUILayout.Button ("Remove"))
                {
                    if (property.FieldList.Count > 0)
                    {
                        property.FieldList.RemoveAt (property.FieldList.Count - 1);
                        serializedObject.Update ();
                    }
                }

                if (GUILayout.Button ("Clear"))
                {
                    property.FieldList.Clear ();
                    serializedObject.Update ();
                }
            });

            serializedObject.ApplyModifiedProperties ();
        }

        public static void DrawPropertyFieldInput (SerializedProperty serializedField)
        {
            SerializedProperty serializedType = serializedField.FindPropertyRelative ("m_type");
            serializedType.enumValueIndex = (int) MyEditorLayout.EnumSelectionPopup ((PropertyFieldType) serializedType.enumValueIndex);

            PropertyFieldType type = (PropertyFieldType) serializedType.enumValueIndex;

            switch (type)
            {
                case PropertyFieldType.Int:
                {
                    var serializedValue = serializedField.FindPropertyRelative ("m_intValue");
                    serializedValue.intValue = EditorGUILayout.IntField (serializedValue.intValue);
                    break;
                }

                case PropertyFieldType.Float:
                {
                    var serializedValue = serializedField.FindPropertyRelative ("m_floatValue");
                    serializedValue.floatValue = EditorGUILayout.FloatField (serializedValue.floatValue);
                    break;
                }

                case PropertyFieldType.Bool:
                {
                    var serializedValue = serializedField.FindPropertyRelative ("m_boolValue");
                    serializedValue.boolValue = EditorGUILayout.Toggle (serializedValue.boolValue);
                    break;
                }

                case PropertyFieldType.String:
                {
                    var serializedValue = serializedField.FindPropertyRelative ("m_stringValue");
                    serializedValue.stringValue = EditorGUILayout.TextField (serializedValue.stringValue);
                    break;
                }

                case PropertyFieldType.Vector2:
                {
                    var serializedValue = serializedField.FindPropertyRelative ("m_vector2Value");
                    serializedValue.vector2Value = EditorGUILayout.Vector2Field (string.Empty, serializedValue.vector2Value);
                    break;
                }

                case PropertyFieldType.Vector3:
                {
                    var serializedValue = serializedField.FindPropertyRelative ("m_vector3Value");
                    serializedValue.vector3Value = EditorGUILayout.Vector3Field (string.Empty, serializedValue.vector3Value);
                    break;
                }

                case PropertyFieldType.Vector4:
                {
                    var serializedValue = serializedField.FindPropertyRelative ("m_vector4Value");
                    serializedValue.vector4Value = EditorGUILayout.Vector4Field (string.Empty, serializedValue.vector4Value);
                    break;
                }

                case PropertyFieldType.Quaternion:
                {
                    var serializedValue = serializedField.FindPropertyRelative ("m_quaternionValue");
                    Vector4 quaternionValue = new Vector4 (serializedValue.quaternionValue.x, serializedValue.quaternionValue.y, serializedValue.quaternionValue.z, serializedValue.quaternionValue.w);
                    quaternionValue = EditorGUILayout.Vector4Field (string.Empty, quaternionValue);
                    serializedValue.quaternionValue = new Quaternion (quaternionValue.x, quaternionValue.y, quaternionValue.z, quaternionValue.w);
                    break;
                }
            }
        }
    }
}