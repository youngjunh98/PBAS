using System;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

namespace PBAS
{
    public class MyEditorUtility
    {
        public static int ShowObjectPicker<TEnum> (Object firstPick, bool bAllowSceneObject, string searchFilter) where TEnum : Object
        {
            int pickerID = GUIUtility.GetControlID (FocusType.Passive);
            EditorGUIUtility.ShowObjectPicker<TEnum> (firstPick, bAllowSceneObject, searchFilter, pickerID);

            return pickerID;
        }

        public static void ForEachSerializedArray (string name, SerializedObject serializedObject, Action<SerializedProperty, int> action)
        {
            if (string.IsNullOrEmpty (name))
            {
                return;
            }

            ForEachSerializedArray (serializedObject.FindProperty (name), action);
        }

        public static void ForEachSerializedArray (string name, SerializedProperty serializedProperty, Action<SerializedProperty, int> action)
        {
            if (string.IsNullOrEmpty (name))
            {
                return;
            }

            ForEachSerializedArray (serializedProperty.FindPropertyRelative (name), action);
        }

        public static void ForEachSerializedArray (SerializedProperty serializedArray, Action<SerializedProperty, int> action)
        {
            if (serializedArray == null || action == null)
            {
                return;
            }

            if (serializedArray.isArray)
            {
                for (int i = 0; i < serializedArray.arraySize; i++)
                {
                    action.Invoke (serializedArray.GetArrayElementAtIndex (i), i);
                }
            }
        }

        public static void AddSerializedArray<TElement> (SerializedProperty serializedArray, TElement value) where TElement : Object, new()
        {
            if (serializedArray.isArray)
            {
                int insertIndex = serializedArray.arraySize;

                serializedArray.InsertArrayElementAtIndex (insertIndex);
                serializedArray.GetArrayElementAtIndex (insertIndex).objectReferenceValue = value ? value : new TElement ();
            }
        }

        public static void RemoveSerializedArray (SerializedProperty serializedArray)
        {
            RemoveAtSerializedArray (serializedArray, serializedArray.arraySize - 1);
        }

        public static void RemoveAtSerializedArray (SerializedProperty serializedArray, int removeIndex)
        {
            if (serializedArray.isArray)
            {
                if (removeIndex >= 0 && removeIndex < serializedArray.arraySize)
                {
                    var removeTarget = serializedArray.GetArrayElementAtIndex (removeIndex);

                    if (removeTarget.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        removeTarget.objectReferenceValue = null;
                    }

                    serializedArray.DeleteArrayElementAtIndex (removeIndex);
                }
            }
        }

        public static void ClearSerializedArray (SerializedProperty serializedArray)
        {
            if (serializedArray.isArray)
            {
                serializedArray.ClearArray ();
            }
        }
    }
}
