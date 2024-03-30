namespace FSM
{   
    /// <summary>
    /// Condition for And
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AndConditionNode<T> : FSMConditionNode<T>
    {
        private FSMConditionNode<T> con1;
        private FSMConditionNode<T> con2;
        
        public AndConditionNode(FSMConditionNode<T> con1, FSMConditionNode<T> con2)
        {
            this.con1 = con1;
            this.con2 = con2;
        }

        public override bool Condition(T owner)
        {
            return con1.Condition(owner) && con2.Condition(owner);
        }
    }
}