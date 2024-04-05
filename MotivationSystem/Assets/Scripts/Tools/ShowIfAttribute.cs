using AnimSystem.Core;
using UnityEngine;

namespace Tools
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionFieldName { get; private set; }
        public Type ShowIfConditionIsTrue { get; private set; }

        public ShowIfAttribute(string attributeName, Type showIfCondition)
        {
            this.ConditionFieldName = attributeName;
            this.ShowIfConditionIsTrue = showIfCondition;
        }
    }
}