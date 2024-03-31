using System;
using System.Collections.Generic;
using System.Linq;

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

        public void BindSwitchHandle(Action<T> handle)
        {
            onExitStateHandle += handle;
        }
        
        public void OnEnter(T owner)
        {
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