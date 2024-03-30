using System;

namespace FSM
{
    /// <summary>
    /// Parent Condition for And, Or, Not
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FSMConditionNode<T>
    {
        private Func<T, bool> myConditionHandler;
        public int id;
        public FSMConditionNode()
        {
        }

        public FSMConditionNode(Func<T, bool> handle,int id)
        {
            myConditionHandler = handle;
            this.id = id;
        }

        public virtual bool Condition(T owner)
        {
            return myConditionHandler != null && myConditionHandler.Invoke(owner);
        }

        public static FSMConditionNode<T> operator &(FSMConditionNode<T> con1, FSMConditionNode<T> con2)
        {
            return new AndConditionNode<T>(con1, con2);
        }

        public static FSMConditionNode<T> operator |(FSMConditionNode<T> con1, FSMConditionNode<T> con2)
        {
            return new OrConditionNode<T>(con1, con2);
        }

        public static FSMConditionNode<T> operator !(FSMConditionNode<T> con)
        {
            return new NotConditionNode<T>(con);
        }
        
    }
}