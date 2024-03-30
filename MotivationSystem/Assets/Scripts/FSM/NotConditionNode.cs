namespace FSM
{
    public class NotConditionNode<T> : FSMConditionNode<T>
    {
        private FSMConditionNode<T> con1;
               
        public NotConditionNode(FSMConditionNode<T> con1)
        {
            this.con1 = con1;
        }

        public override bool Condition(T owner)
        {
            return !con1.Condition(owner);
        }
    }
}