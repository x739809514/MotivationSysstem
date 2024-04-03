using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FSM
{
    public class FSMStateNode<T>
    {
        private Action<T> onEnterHandle;
        private Action<T> onUpdateHandle;
        private Action<T> onExitStateHandle;
        public string stateName;
        public Dictionary<int, FSMConditionNode<T>> conditionDic;

        public FSMStateNode(string stateName)
        {
            this.stateName = stateName;
        }


#region Conditions

        public void AddConditions(FSMConditionNode<T> conditionNode)
        {
            if (conditionDic==null)
            {
                conditionDic = new Dictionary<int, FSMConditionNode<T>>();
            }

            if (conditionDic.Keys.Contains(conditionNode.id)==false)
            {
                conditionDic.Add(conditionNode.id,conditionNode);
            }
        }

        public void RemoveCondition(FSMConditionNode<T> conditionNode)
        {
            if (conditionDic==null)
            {
                return;
            }

            if (conditionDic.ContainsKey(conditionNode.id))
            {
                conditionDic.Remove(conditionNode.id);
            }
        }

        public bool CheckCondition(T owner)
        {
            if (conditionDic==null)
            {
                return false;
            }

            foreach (var conditionNode in conditionDic)
            {
                if (conditionNode.Value.Condition(owner)==false)
                {
                    return false;
                }
            }

            return true;
        }

#endregion
        

#region Handles

        public void BindEnterHandle(Action<T> handle)
        {
            onEnterHandle += handle;
        }

        public void BindUpdateHandle(Action<T> handle)
        {
            onUpdateHandle += handle;
        }

        public void BindExitHandle(Action<T> handle)
        {
            onExitStateHandle += handle;
        }
        
        public void OnEnter(T owner)
        {
            Debug.Log("<color=green>Now is "+stateName+"</color>");
            onEnterHandle?.Invoke(owner);    
        }

        public void OnUpdate(T owner)
        {
            onUpdateHandle?.Invoke(owner);
        }

        public void OnExitState(T owner)
        {
            onExitStateHandle?.Invoke(owner);
        }

#endregion
    }
}