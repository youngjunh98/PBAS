using System;
using UnityEngine;
using UnityEditor;

namespace PBAS
{
    public class MyEditorLayout
    {
        public static void Horizontal (System.Action onHorizontal)
        {
            EditorGUILayout.BeginHorizontal ();
            onHorizontal ();
            EditorGUILayout.EndHorizontal ();
        }

        public static void Vertical (System.Action onVertical)
        {
            EditorGUILayout.BeginVertical ();
            onVertical ();
            EditorGUILayout.EndVertical ();
        }

        public static void FoldoutHeaderGroup (string header, ref bool bShowGroup, System.Action onGroup)
        {
            bShowGroup = EditorGUILayout.BeginFoldoutHeaderGroup (bShowGroup, header);

            if (bShowGroup)
            {
                onGroup.Invoke ();
            }

            EditorGUILayout.EndFoldoutHeaderGroup ();
        }

        public static void HorizontalLine (float thickness = 1.0f)
        {
            EditorGUILayout.Separator ();
            EditorGUI.DrawRect (EditorGUILayout.GetControlRect (false, thickness), Color.gray);
            EditorGUILayout.Separator ();
        }

        public static TEnum EnumSelectionPopup<TEnum> (TEnum enumValue) where TEnum : Enum
        {
            return (TEnum) EditorGUILayout.EnumPopup (enumValue);
        }
    }
}
