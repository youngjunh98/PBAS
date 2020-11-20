using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PBAS
{
    [CustomEditor (typeof (PBASAnimator))]
    public class PBASAnimatorEditor : Editor
    {
        private int m_pickerID = -1;

        public override void OnInspectorGUI ()
        {
            PBASAnimator actionAnimator = target as PBASAnimator;
            List<Action> removeList = new List<Action> ();

            if (Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID () == m_pickerID)
            {
                Object pickedObject = EditorGUIUtility.GetObjectPickerObject ();

                if (pickedObject)
                {
                    var pickedAction = pickedObject as Action;

                    if (actionAnimator.CommandDictionary.ContainsKey (pickedAction) == false)
                    {
                        actionAnimator.CommandDictionary.Add (pickedAction, new AnimationCommandList ());
                    }
                }

                m_pickerID = -1;
            }

            foreach (var pair in actionAnimator.CommandDictionary)
            {
                MyEditorLayout.Vertical (() =>
                {
                    MyEditorLayout.Horizontal (() =>
                    {
                        Action action = pair.Key;
                        GUILayout.Label (action.name, GUILayout.ExpandWidth (false));

                        if (GUILayout.Button ("X", GUILayout.ExpandWidth (false)))
                        {
                            removeList.Add (action);
                        }
                    });


                    AnimationCommandList commandList = pair.Value;

                    foreach (AnimationCommand command in commandList)
                    {
                        MyEditorLayout.Horizontal (() =>
                        {
                            command.Operation = MyEditorLayout.EnumSelectionPopup (command.Operation);
                            command.TargetParameterName = EditorGUILayout.TextField (command.TargetParameterName);

                            switch (command.Operation)
                            {
                                case AnimationCommandOperation.SetInt:
                                {
                                    command.IntValue = EditorGUILayout.IntField (command.IntValue);
                                    break;
                                }

                                case AnimationCommandOperation.SetFloat:
                                {
                                    command.FloatValue = EditorGUILayout.FloatField (command.FloatValue);
                                    break;
                                }

                                case AnimationCommandOperation.SetBool:
                                {
                                    command.BoolValue = EditorGUILayout.Toggle (command.BoolValue);
                                    break;
                                }
                            }
                        });
                    }

                    MyEditorLayout.Horizontal (() =>
                    {
                        if (GUILayout.Button ("Add Command"))
                        {
                            var command = new AnimationCommand ();
                            command.TargetParameterName = "Enter target parameter name";
                            command.Operation = AnimationCommandOperation.FireTrigger;

                            commandList.List.Add (command); ;
                        }

                        if (GUILayout.Button ("Remove Command"))
                        {
                            if (commandList.List.Any ())
                            {
                                commandList.List.RemoveAt (commandList.List.Count - 1);
                            }
                        }
                    });
                });

                MyEditorLayout.HorizontalLine ();
            }

            foreach (var remove in removeList)
            {
                actionAnimator.CommandDictionary.Remove (remove);
            }

            if (GUILayout.Button ("Add") && m_pickerID < 0)
            {
                m_pickerID = GUIUtility.GetControlID (FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<Action> (null, false, string.Empty, m_pickerID);
            }

            serializedObject.ApplyModifiedProperties ();
        }
    }
}
