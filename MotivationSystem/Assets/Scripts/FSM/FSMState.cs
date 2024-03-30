using System;
using System.Collections.Generic;
using System.Linq;

namespace FSM
{
    public class FSMState<T>
    {
        public Action<T> onEnterHandle;
        public Action<T> onUpdateHandle;
        public Action<T> onSwitchStateHandle;
        
        public Dictionary<int, FSMConditionNode<T>> conditionDic;

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

        public List<FSMConditionNode<T>> CheckEnableCondition(T owner)
        {
            if (conditionDic==null)
            {
                return null;
            }

            List<FSMConditionNode<T>> list = new List<FSMConditionNode<T>>();
            foreach (var conditionNode in conditionDic)
            {
                if (conditionNode.Value.Condition(owner))
                {
                    list.Add(conditionNode.Value);
                }
            }

            return list;
        }
        
        public void OnEnter(T owner)
        {
            onEnterHandle?.Invoke(owner);    
        }

        public void OnUpdate(T owner)
        {
            onUpdateHandle?.Invoke(owner);
        }

        public void OnSwitchState(T owner)
        {
            onSwitchStateHandle?.Invoke(owner);
        }
    }
}