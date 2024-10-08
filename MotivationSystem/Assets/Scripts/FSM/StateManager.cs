using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public class StateManager<T>
    {
        public Dictionary<string, FSMStateNode<T>> stateDic;
        public T owner;
        public FSMStateNode<T> defaultState;
        public FSMStateNode<T> curState;

        public StateManager(T owner)
        {
            this.owner = owner;
        }

        public void SetDefaultState(string stateName)
        {
            if (stateDic == null || stateDic.TryGetValue(stateName, out var state) == false)
            {
                return;
            }

            defaultState = state;
            curState = state;
            curState.OnEnter(owner);
        }

        public void AddState(FSMStateNode<T> stateNode)
        {
            stateDic ??= new Dictionary<string, FSMStateNode<T>>();

            stateDic.TryAdd(stateNode.stateName, stateNode);
        }

        public bool SwitchState(FSMStateNode<T> newState)
        {
            if (stateDic.TryGetValue(newState.stateName, out var state))
            {
                if (newState.CheckCondition(owner)==false)
                {
                    Debug.LogError("Condition is not accessed!---"+newState.stateName);
                    return false;
                }
                if (curState != null)
                {
                    curState.OnExitState(owner);
                }

                newState.OnEnter(owner);
                curState = newState;
                return true;
            }
            Debug.LogError("No this state!---"+newState.stateName);
            return false;
        }

        public void UpdateState()
        {
            if (stateDic == null)
            {
                return;
            }

            if (curState != null)
            {
                curState.OnUpdate(owner);
            }
        }

        public void ExitState()
        {
            curState.OnExitState(owner);
        }
    }
}