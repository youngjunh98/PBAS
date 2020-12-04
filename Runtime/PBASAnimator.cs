using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBAS
{
    public enum AnimationCommandOperation
    {
        FireTrigger, SetInt, SetFloat, SetBool
    }

    [Serializable]
    public class AnimationCommand
    {
        [SerializeField]
        private AnimationCommandOperation m_operation;
        [SerializeField]
        private string m_targetParameterName;
        [SerializeField]
        private int m_intValue;
        [SerializeField]
        private float m_floatValue;
        [SerializeField]
        private bool m_boolValue;

        public AnimationCommandOperation Operation
        {
            get => m_operation;
            set => m_operation = value;
        }

        public string TargetParameterName
        {
            get => m_targetParameterName;
            set => m_targetParameterName = value;
        }

        public int IntValue
        {
            get => m_intValue;
            set => m_intValue = value;
        }

        public float FloatValue
        {
            get => m_floatValue;
            set => m_floatValue = value;
        }

        public bool BoolValue
        {
            get => m_boolValue;
            set => m_boolValue = value;
        }
    }

    [Serializable]
    public class AnimationCommandList : IEnumerable<AnimationCommand>
    {
        [SerializeField]
        private List<AnimationCommand> m_list = new List<AnimationCommand> ();

        public List<AnimationCommand> List
        {
            get => m_list;
        }

        public IEnumerator<AnimationCommand> GetEnumerator ()
        {
            return List.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return List.GetEnumerator ();
        }
    }

    [Serializable]
    public class AnimationCommandDictionary : SerializableDictionary<Action, AnimationCommandList>
    {
    }

    [RequireComponent (typeof (Animator))]
    public class PBASAnimator : MonoBehaviour
    {
        [SerializeField]
        private AnimationCommandDictionary m_commandDict = new AnimationCommandDictionary ();

        private PBASActor m_actor;
        private int m_currentStateHash;
        private bool m_bRegisterListener;
        private StateEventDispatcher m_stateEventDispatcher;

        private Animator Animator
        {
            get;
            set;
        }

        public AnimationCommandDictionary CommandDictionary
        {
            get => m_commandDict;
        }

        private void Awake ()
        {
            Animator = GetComponent<Animator> ();

            m_actor = GetComponent<PBASActor> ();
            m_stateEventDispatcher = Animator.GetBehaviour<StateEventDispatcher> ();
        }

        private void LateUpdate ()
        {
            int nextStateHash = Animator.GetNextAnimatorStateInfo (0).shortNameHash;

            if (m_bRegisterListener)
            {
                if (m_currentStateHash != nextStateHash)
                {
                    m_currentStateHash = nextStateHash;
                    m_stateEventDispatcher.AddListener (m_currentStateHash, StateEventName.Exit, FinishActionAnimation);
                }
                else
                {
                    m_actor.FinishAction (false);
                }

                m_bRegisterListener = false;
            }
        }

        public void PlayActionAnimation (Action action)
        {
            if (!Animator)
            {
                return;
            }

            AnimationCommandList commandList;

            if (CommandDictionary.TryGetValue (action, out commandList) == false)
            {
                return;
            }

            foreach (AnimationCommand command in commandList)
            {
                switch (command.Operation)
                {
                    case AnimationCommandOperation.FireTrigger:
                    {
                        Animator.SetTrigger (command.TargetParameterName);
                        break;
                    }
                    case AnimationCommandOperation.SetInt:
                    {
                        Animator.SetInteger (command.TargetParameterName, command.IntValue);
                        break;
                    }

                    case AnimationCommandOperation.SetFloat:
                    {
                        Animator.SetFloat (command.TargetParameterName, command.FloatValue);
                        break;
                    }

                    case AnimationCommandOperation.SetBool:
                    {
                        Animator.SetBool (command.TargetParameterName, command.BoolValue);
                        break;
                    }
                }
            }

            m_currentStateHash = Animator.GetCurrentAnimatorStateInfo (0).shortNameHash;
            m_bRegisterListener = true;
        }

        private void FinishActionAnimation (AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_stateEventDispatcher.RemoveListener (m_currentStateHash, StateEventName.Exit, FinishActionAnimation);
            m_actor.FinishAction (true);
        }    
    }
}
