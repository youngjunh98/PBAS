using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PBAS
{
    [CustomEditor (typeof (Action))]
    public class ActionEditor : Editor
    {
        private bool m_bIsShowObjectPicker;
        private int m_objectPickerControlId;
        private int m_objectPickerRequestIndex;
        private bool m_bRequiredPropertyPicker;

        private bool m_bShowParams = true;
        private bool m_bShowPreconditions = true;
        private bool m_bShowEffects = true;

        public override void OnInspectorGUI ()
        {
            MyEditorLayout.FoldoutHeaderGroup ("Parameters", ref m_bShowParams, () =>
            {
                DrawParameterGUI ();
            });

            EditorGUILayout.Separator ();

            MyEditorLayout.FoldoutHeaderGroup ("Preconditions", ref m_bShowPreconditions, () =>
            {
                DrawPreconditionGUI ();
            });

            EditorGUILayout.Separator ();

            MyEditorLayout.FoldoutHeaderGroup ("Effects", ref m_bShowEffects, () =>
            {
                DrawEffectGUI ();
            });

            serializedObject.ApplyModifiedProperties ();
        }

        private void DrawListEditButtons<TElement> (List<TElement> list) where TElement : new()
        {
            MyEditorLayout.Horizontal (() =>
            {
                if (GUILayout.Button ("Add"))
                {
                    list.Add (new TElement ());
                    serializedObject.Update ();
                }

                if (GUILayout.Button ("Remove"))
                {
                    if (list.Count > 0)
                    {
                        list.RemoveAt (list.Count - 1);
                        serializedObject.Update ();
                    }
                }

                if (GUILayout.Button ("Clear"))
                {
                    list.Clear ();
                    serializedObject.Update ();
                }
            });
        }

        private void DrawParameterGUI ()
        {
            var action = serializedObject.targetObject as Action;
            var serializedParameterList = serializedObject.FindProperty ("m_parameterList");

            if (m_bIsShowObjectPicker && Event.current.commandName == "ObjectSelectorClosed" && m_objectPickerControlId == EditorGUIUtility.GetObjectPickerControlID ())
            {
                var selectedProperty = EditorGUIUtility.GetObjectPickerObject () as Property;

                if (selectedProperty)
                {
                    var serializedParameter = serializedParameterList.GetArrayElementAtIndex (m_objectPickerRequestIndex);
                    var serializedRequiredPropertyList = serializedParameter.FindPropertyRelative ("m_requiredPropertyList");
                    var serializedForbiddendPropertyList = serializedParameter.FindPropertyRelative ("m_forbiddenPropertyList");
                    var targetPropertyList = m_bRequiredPropertyPicker ? serializedRequiredPropertyList : serializedForbiddendPropertyList;
                    int insertIndex = targetPropertyList.arraySize;

                    MyEditorUtility.AddSerializedArray (targetPropertyList, selectedProperty);
                    serializedObject.ApplyModifiedProperties ();
                }

                m_bIsShowObjectPicker = false;
            }

            MyEditorUtility.ForEachSerializedArray (serializedParameterList, (serializedParameter, index) =>
            {
                DrawParameter (serializedParameter, index);
                MyEditorLayout.HorizontalLine ();
            });

            DrawListEditButtons (action.ParameterList);
        }

        private void DrawParameter (SerializedProperty serializedParameter, int index)
        {
            EditorGUILayout.PropertyField (serializedParameter.FindPropertyRelative ("m_name"));
            EditorGUILayout.Space ();

            var serializedRequiredPropertyList = serializedParameter.FindPropertyRelative ("m_requiredPropertyList");
            var serializedForbiddendPropertyList = serializedParameter.FindPropertyRelative ("m_forbiddenPropertyList");

            MyEditorLayout.Horizontal (() =>
            {
                // Required Property
                MyEditorLayout.Vertical (() =>
                {
                    EditorGUILayout.LabelField ("Requried Properties", EditorStyles.boldLabel);
                    DrawParameterPropertyList (serializedRequiredPropertyList, index, true);
                });

                // Forbidden Property
                MyEditorLayout.Vertical (() =>
                {
                    EditorGUILayout.LabelField ("Forbbiden Properties", EditorStyles.boldLabel);
                    DrawParameterPropertyList (serializedForbiddendPropertyList, index, false);
                });
            });
        }

        private void DrawParameterPropertyList (SerializedProperty serializedPropertyList, int index, bool bRequiredProperty)
        {
            MyEditorUtility.ForEachSerializedArray (serializedPropertyList, (serializedProperty, i) =>
            {
                var property = serializedProperty.objectReferenceValue;
                var propertyName = property ? property.name : "None";

                EditorGUILayout.LabelField (propertyName);
            });

            MyEditorLayout.Horizontal (() =>
            {
                if (GUILayout.Button ("+") && m_bIsShowObjectPicker == false)
                {
                    m_bIsShowObjectPicker = true;
                    m_objectPickerControlId = MyEditorUtility.ShowObjectPicker<Property> (null, false, "");
                    m_objectPickerRequestIndex = index;
                    m_bRequiredPropertyPicker = bRequiredProperty;

                    EditorGUIUtility.ShowObjectPicker<Property> (null, false, string.Empty, m_objectPickerControlId);
                }

                if (GUILayout.Button ("-"))
                {
                    MyEditorUtility.RemoveSerializedArray (serializedPropertyList);
                }
            });
        }

        private void DrawPreconditionGUI ()
        {
            var action = serializedObject.targetObject as Action;
            var preconditionList = serializedObject.FindProperty ("m_preconditionList");

            MyEditorUtility.ForEachSerializedArray (preconditionList, (serializedPrecondition, index) =>
            {
                MyEditorLayout.Horizontal (() =>
                {
                    // Left parameter
                    var lParamIndex = serializedPrecondition.FindPropertyRelative ("m_leftParameterIndex");
                    var lParamPropertyIndex = serializedPrecondition.FindPropertyRelative ("m_leftPropertyIndex");
                    var lParamFieldIndex = serializedPrecondition.FindPropertyRelative ("m_leftFieldIndex");

                    DrawPreconditionParameter (action, serializedPrecondition, lParamIndex, lParamPropertyIndex, lParamFieldIndex, false);

                    if (lParamIndex.intValue >= 0 && lParamIndex.intValue < action.ParameterList.Count)
                    {
                        Action.Parameter parameter = action.ParameterList[lParamIndex.intValue];

                        if (lParamPropertyIndex.intValue >= 0 && lParamPropertyIndex.intValue < parameter.RequiredPropertyList.Count)
                        {
                            Property property = parameter.RequiredPropertyList[lParamPropertyIndex.intValue];

                            if (lParamFieldIndex.intValue >= 0 && lParamFieldIndex.intValue < property.FieldList.Count)
                            {
                                // Comparison
                                var comparison = serializedPrecondition.FindPropertyRelative ("m_comparison");
                                comparison.enumValueIndex = (int) MyEditorLayout.EnumSelectionPopup ((Action.EPreconditionComparison) comparison.enumValueIndex);

                                // Right parameter
                                var rParamIndex = serializedPrecondition.FindPropertyRelative ("m_rightParameterIndex");
                                var rParamPropertyIndex = serializedPrecondition.FindPropertyRelative ("m_rightPropertyIndex");
                                var rParamFieldIndex = serializedPrecondition.FindPropertyRelative ("m_rightFieldIndex");

                                DrawPreconditionParameter (action, serializedPrecondition, rParamIndex, rParamPropertyIndex, rParamFieldIndex, true);
                            }
                        }
                    }
                });
            });

            DrawListEditButtons (action.PreconditionList);
        }

        private void DrawPreconditionParameter (Action action, SerializedProperty serializedPrecondition, SerializedProperty serializedParamter, SerializedProperty serializedProperty, SerializedProperty serializedField, bool isRightParam)
        {
            // Parameter
            var parameterNames = action?.ParameterNames ?? new List<string> ();
            bool bAnyParameter = parameterNames?.Any () ?? false;

            if (bAnyParameter == false && isRightParam == false)
            {
                parameterNames.Add ("None");
            }

            if (isRightParam)
            {
                parameterNames.Add ("Constant");
            }

            int selectedParameterIndex = 0;

            if (serializedParamter.intValue >= 0 && serializedParamter.intValue < parameterNames.Count)
            {
                selectedParameterIndex = serializedParamter.intValue;
            }

            if (serializedParamter.intValue < 0)
            {
                selectedParameterIndex = parameterNames.IndexOf ("Constant");
            }

            selectedParameterIndex = EditorGUILayout.Popup (selectedParameterIndex, parameterNames.ToArray ());
            serializedParamter.intValue = selectedParameterIndex;

            if (isRightParam && selectedParameterIndex == parameterNames.IndexOf ("Constant"))
            {
                serializedParamter.intValue = -1;

                var lParamIndex = serializedPrecondition.FindPropertyRelative ("m_leftParameterIndex");
                var lParamPropertyIndex = serializedPrecondition.FindPropertyRelative ("m_leftPropertyIndex");
                var lParamFieldIndex = serializedPrecondition.FindPropertyRelative ("m_leftFieldIndex");
                var leftType = action.ParameterList[lParamIndex.intValue].RequiredPropertyList[lParamPropertyIndex.intValue].FieldList[lParamFieldIndex.intValue].DefaultValue.Type;

                var rightValue = serializedPrecondition.FindPropertyRelative ("m_rightValue");
                var rightValueType = rightValue.FindPropertyRelative ("m_type");
                rightValueType.enumValueIndex = (int) leftType;

                PropertyEditor.DrawPropertyFieldInput (rightValue);
            }
            else
            {
                var parameter = bAnyParameter ? action.ParameterList[selectedParameterIndex] : null;

                // Property
                var propertyNames = parameter?.RequiredPropertyNames ?? new List<string> ();
                bool bAnyProperty = propertyNames?.Any () ?? false;

                if (bAnyProperty == false)
                {
                    propertyNames.Add ("None");
                }

                int selectedPropertyIndex = 0;

                if (serializedProperty.intValue >= 0 && serializedProperty.intValue < propertyNames.Count)
                {
                    selectedPropertyIndex = serializedProperty.intValue;
                }

                selectedPropertyIndex = EditorGUILayout.Popup (selectedPropertyIndex, propertyNames.ToArray ());
                serializedProperty.intValue = selectedPropertyIndex;

                var property = bAnyProperty ? parameter.RequiredPropertyList[selectedPropertyIndex] : null;

                // Field
                var fieldNames = property?.FieldNames ?? new List<string> ();
                bool bAnyField = fieldNames?.Any () ?? false;

                if (bAnyField == false)
                {
                    fieldNames.Add ("None");
                }

                int selectedFieldIndex = 0;

                if (serializedField.intValue >= 0 && serializedField.intValue < fieldNames.Count)
                {
                    selectedFieldIndex = serializedField.intValue;
                }

                selectedFieldIndex = EditorGUILayout.Popup (selectedFieldIndex, fieldNames.ToArray ());
                serializedField.intValue = selectedFieldIndex;
            }
        }

        private void DrawEffectGUI ()
        {
            var action = serializedObject.targetObject as Action;
            var serializedEffectList = serializedObject.FindProperty ("m_effectList");

            MyEditorUtility.ForEachSerializedArray (serializedEffectList, (serializedEffect, index) =>
            {
                var operation = serializedEffect.FindPropertyRelative ("m_operation");
                var operationEnum = (Action.EEffectOperation) operation.enumValueIndex;

                MyEditorLayout.Horizontal (() =>
                {
                    EditorGUILayout.LabelField ("Operation");

                    // Operation
                    operationEnum = MyEditorLayout.EnumSelectionPopup (operationEnum);
                    operation.enumValueIndex = (int) operationEnum;
                });

                MyEditorLayout.Horizontal (() =>
                {
                    // Target parameter
                    var setParameterIndex = serializedEffect.FindPropertyRelative ("m_setParameterIndex");
                    var parameterNames = action?.ParameterNames ?? new List<string> ();
                    bool bAnyParameter = parameterNames?.Any () ?? false;

                    if (bAnyParameter == false)
                    {
                        parameterNames.Add ("None");
                    }

                    int selectedParameterIndex = 0;

                    if (setParameterIndex.intValue >= 0 && setParameterIndex.intValue < parameterNames.Count)
                    {
                        selectedParameterIndex = setParameterIndex.intValue;
                    }

                    selectedParameterIndex = EditorGUILayout.Popup (selectedParameterIndex, parameterNames.ToArray ());
                    setParameterIndex.intValue = selectedParameterIndex;

                    var parameter = bAnyParameter ? action.ParameterList[selectedParameterIndex] : null;

                    switch (operationEnum)
                    {
                        case Action.EEffectOperation.SetValue:
                        {
                            // Property
                            var propertyIndex = serializedEffect.FindPropertyRelative ("m_setPropertyIndex");
                            var propertyNames = parameter?.RequiredPropertyNames ?? new List<string> ();
                            bool bAnyProperty = propertyNames?.Any () ?? false;

                            if (bAnyProperty == false)
                            {
                                propertyNames.Add ("None");
                            }

                            int selectedPropertyIndex = 0;

                            if (propertyIndex.intValue >= 0 && propertyIndex.intValue < propertyNames.Count)
                            {
                                selectedPropertyIndex = propertyIndex.intValue;
                            }

                            selectedPropertyIndex = EditorGUILayout.Popup (selectedPropertyIndex, propertyNames.ToArray ());
                            propertyIndex.intValue = selectedPropertyIndex;

                            var property = bAnyProperty ? parameter.RequiredPropertyList[selectedPropertyIndex] : null;

                            // Field
                            var fieldIndex = serializedEffect.FindPropertyRelative ("m_setFieldIndex");
                            var fieldNames = property?.FieldNames ?? new List<string> ();
                            bool bAnyField = fieldNames?.Any () ?? false;

                            if (bAnyField == false)
                            {
                                fieldNames.Add ("None");
                            }

                            int selectedFieldIndex = 0;

                            if (fieldIndex.intValue >= 0 && fieldIndex.intValue < fieldNames.Count)
                            {
                                selectedFieldIndex = fieldIndex.intValue;
                            }

                            selectedFieldIndex = EditorGUILayout.Popup (selectedFieldIndex, fieldNames.ToArray ());
                            fieldIndex.intValue = selectedFieldIndex;

                            var field = bAnyField ? property.FieldList[selectedFieldIndex] : null;

                            // Field Value
                            if (field != null)
                            {
                                GUILayout.Label ("=", EditorStyles.boldLabel, GUILayout.ExpandWidth (false));

                                var setFieldValue = serializedEffect.FindPropertyRelative ("m_setFieldValue");
                                setFieldValue.FindPropertyRelative ("m_type").enumValueIndex = (int) field.DefaultValue.Type;

                                PropertyEditor.DrawPropertyFieldInput (setFieldValue);
                            }

                            break;
                        }

                        case Action.EEffectOperation.AddProperty:
                        {
                            var addProperty = serializedEffect.FindPropertyRelative ("m_targetProperty");
                            EditorGUILayout.PropertyField (addProperty, new GUIContent (""));

                            break;
                        }

                        case Action.EEffectOperation.RemoveProperty:
                        {
                            // Property
                            var propertyIndex = serializedEffect.FindPropertyRelative ("m_setPropertyIndex");
                            var propertyNames = parameter?.RequiredPropertyNames ?? new List<string> ();
                            bool bAnyProperty = propertyNames?.Any () ?? false;

                            if (bAnyProperty == false)
                            {
                                propertyNames.Add ("None");
                            }

                            int selectedPropertyIndex = 0;

                            if (propertyIndex.intValue >= 0 && propertyIndex.intValue < propertyNames.Count)
                            {
                                selectedPropertyIndex = propertyIndex.intValue;
                            }

                            selectedPropertyIndex = EditorGUILayout.Popup (selectedPropertyIndex, propertyNames.ToArray ());
                            propertyIndex.intValue = selectedPropertyIndex;

                            break;
                        }
                    }
                });


                MyEditorLayout.HorizontalLine ();
            });

            DrawListEditButtons (action.EffectList);
        }
    }
}
