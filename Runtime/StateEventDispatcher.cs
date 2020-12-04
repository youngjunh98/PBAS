using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PBAS
{
    [System.Serializable]
    public class StateEvent : UnityEvent<AnimatorStateInfo, int>
    {
    }

    public enum StateEventName
    {
        Enter, Update, Exit
    }

    public class StateEventDispatcher : StateMachineBehaviour
    {
        private Dictionary<int, StateEvent> m_enterEventMap = new Dictionary<int, StateEvent> ();
        private Dictionary<int, StateEvent> m_updateEventMap = new Dictionary<int, StateEvent> ();
        private Dictionary<int, StateEvent> m_exitEventMap = new Dictionary<int, StateEvent> ();

        public void AddListener (string stateName, StateEventName eventName, UnityAction<AnimatorStateInfo, int> listener)
        {
            AddListener (Animator.StringToHash (stateName), eventName, listener);
        }

        public void RemoveListener (string stateName, StateEventName eventName, UnityAction<AnimatorStateInfo, int> listener)
        {
            RemoveListener (Animator.StringToHash (stateName), eventName, listener);
        }

        public void AddListener (int stateHash, StateEventName eventName, UnityAction<AnimatorStateInfo, int> listener)
        {
            switch (eventName)
            {
                case StateEventName.Enter:
                {
                    if (m_enterEventMap.ContainsKey (stateHash) == false)
                    {
                        m_enterEventMap.Add (stateHash, new StateEvent ());
                    }

                    m_enterEventMap[stateHash].AddListener (listener);
                    break;
                }

                case StateEventName.Update:
                {
                    if (m_updateEventMap.ContainsKey (stateHash) == false)
                    {
                        m_updateEventMap.Add (stateHash, new StateEvent ());
                    }

                    m_updateEventMap[stateHash].AddListener (listener);
                    break;
                }

                case StateEventName.Exit:
                {
                    if (m_exitEventMap.ContainsKey (stateHash) == false)
                    {
                        m_exitEventMap.Add (stateHash, new StateEvent ());
                    }

                    m_exitEventMap[stateHash].AddListener (listener);
                    break;
                }
            }
        }
        public void RemoveListener (int stateHash, StateEventName eventName, UnityAction<AnimatorStateInfo, int> listener)
        {
            switch (eventName)
            {
                case StateEventName.Enter:
                {
                    if (m_enterEventMap.ContainsKey (stateHash))
                    {
                        m_enterEventMap[stateHash].RemoveListener (listener);
                    }

                    break;
                }

                case StateEventName.Update:
                {
                    if (m_updateEventMap.ContainsKey (stateHash))
                    {
                        m_updateEventMap[stateHash].RemoveListener (listener);
                    }

                    break;
                }

                case StateEventName.Exit:
                {
                    if (m_exitEventMap.ContainsKey (stateHash))
                    {
                        m_exitEventMap[stateHash].RemoveListener (listener);
                    }

                    break;
                }
            }
        }

        private void OnDestroy ()
        {
            foreach (var listener in m_enterEventMap.Values)
            {
                listener.RemoveAllListeners ();
            }

            foreach (var listener in m_updateEventMap.Values)
            {
                listener.RemoveAllListeners ();
            }

            foreach (var listener in m_exitEventMap.Values)
            {
                listener.RemoveAllListeners ();
            }
        }

        // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
        override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var listener in m_enterEventMap)
            {
                if (stateInfo.shortNameHash == listener.Key)
                {
                    listener.Value.Invoke (stateInfo, layerIndex);
                }
            }
        }

        // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
        override public void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var listener in m_updateEventMap)
            {
                if (stateInfo.shortNameHash == listener.Key)
                {
                    listener.Value.Invoke (stateInfo, layerIndex);
                }
            }
        }

        // OnStateExit is called before OnStateExit is called on any state inside this state machine
        override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var listener in m_exitEventMap)
            {
                if (stateInfo.shortNameHash == listener.Key)
                {
                    listener.Value.Invoke (stateInfo, layerIndex);
                }
            }
        }

        // OnStateMove is called before OnStateMove is called on any state inside this state machine
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateIK is called before OnStateIK is called on any state inside this state machine
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMachineEnter is called when entering a state machine via its Entry Node
        //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        //{
        //    
        //}

        // OnStateMachineExit is called when exiting a state machine via its Exit Node
        //override public void OnStateMachineExit (Animator animator, int stateMachinePathHash)
        //{
        //
        //}
    }
}
