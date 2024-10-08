using FSM;

namespace MotionCore
{
    /// <summary>
    /// FSM of player
    /// Add State and Condition here
    /// </summary>
    public class PlayerAI
    {
        private StateManager<PlayerMotion> stateManager;
        private PlayerParam param;

        public FSMStateNode<PlayerMotion> idle { get; }
        public FSMStateNode<PlayerMotion> walk { get; }
        public FSMStateNode<PlayerMotion> run { get; }
        public FSMStateNode<PlayerMotion> land { get; }
        public FSMStateNode<PlayerMotion> attack { get; }
        public FSMStateNode<PlayerMotion> block { get; }
        public FSMStateNode<PlayerMotion> roll { get; }
        public FSMStateNode<PlayerMotion> execution { get; }

        public string curStateName => stateManager.curState?.stateName;

        public PlayerAI(PlayerMotion motion)
        {
            stateManager = new StateManager<PlayerMotion>(motion);
            param = motion.playerParam;

            idle = new FSMStateNode<PlayerMotion>("idle");
            stateManager.AddState(idle);

            walk = new FSMStateNode<PlayerMotion>("walk");
            stateManager.AddState(walk);

            run = new FSMStateNode<PlayerMotion>("run");
            stateManager.AddState(run);

            block = new FSMStateNode<PlayerMotion>("block");
            block.BindEnterHandle(EnterBlockState);
            block.BindExitHandle(ExitBlockState);
            stateManager.AddState(block);

            roll = new FSMStateNode<PlayerMotion>("roll");
            roll.BindEnterHandle(EnterRoll);
            roll.BindExitHandle(ExitRoll);
            stateManager.AddState(roll);

            land = new FSMStateNode<PlayerMotion>("land");
            stateManager.AddState(land);

            attack = new FSMStateNode<PlayerMotion>("attack");
            stateManager.AddState(attack);

            execution = new FSMStateNode<PlayerMotion>("execution");
            execution.BindEnterHandle(EnterExecution);
            execution.BindExitHandle(ExitExecution);
            stateManager.AddState(execution);

            var idleCondition = new FSMConditionNode<PlayerMotion>(p => true, 1000);
            var moveInput = new FSMConditionNode<PlayerMotion>(CheckWalkCondition, 1001);
            var runInput = new FSMConditionNode<PlayerMotion>(CheckRunCondition, 1002);
            var landInput = new FSMConditionNode<PlayerMotion>(CheckLandCondition, 1003);
            var attackInput = new FSMConditionNode<PlayerMotion>(CheckAttackCondition, 1004);
            var blockInput = new FSMConditionNode<PlayerMotion>(CheckBlockCondition, 1005);
            var rollInput = new FSMConditionNode<PlayerMotion>(CheckRollCondition, 1006);
            var executionInput = new FSMConditionNode<PlayerMotion>(CheckExecutionCondition, 1007);

            idle.AddConditions(idleCondition);
            walk.AddConditions(moveInput);
            run.AddConditions(runInput);
            land.AddConditions(landInput);
            attack.AddConditions(attackInput);
            block.AddConditions(blockInput);
            roll.AddConditions(rollInput);
            execution.AddConditions(executionInput);
        }

        public void Update()
        {
            stateManager.UpdateState();
        }

        public bool SwitchState(FSMStateNode<PlayerMotion> state)
        {
            var flag = stateManager.SwitchState(state);
            return flag;
        }


#region Conditions

        private bool CheckWalkCondition(PlayerMotion p)
        {
            return param.InputVal.normalized.magnitude >= 0.1f & param.canMove;
        }

        private bool CheckRunCondition(PlayerMotion p)
        {
            return param.runPress & param.canMove;
        }

        private bool CheckLandCondition(PlayerMotion p)
        {
            return param.velocity.y < 0;
        }

        private bool CheckAttackCondition(PlayerMotion p)
        {
            return param.AttackLevel >= 1 & param.isBlocking==false;
        }

        private bool CheckBlockCondition(PlayerMotion p)
        {
            return param.blockPress & param.isBlocking==false;
        }

        private bool CheckRollCondition(PlayerMotion p)
        {
            return param.rollPress & param.isRolling==false;
        }

        private bool CheckExecutionCondition(PlayerMotion p)
        {
            return true;
        }

#endregion


#region State

        private void EnterBlockState(PlayerMotion obj)
        {
            param.AttackLevel = 0;
            param.canMove = false;
            param.isBlocking = true;
        }
        
        private void ExitBlockState(PlayerMotion obj)
        {
            param.isBlocking = false;
            param.canMove = true;
        }

        private void EnterExecution(PlayerMotion obj)
        {
            param.isBlocking = false;
            param.isExecution = true;
        }

        private void ExitExecution(PlayerMotion obj)
        {
            param.isExecution = false;
            param.canMove = true;
        }

        private void EnterRoll(PlayerMotion obj)
        {
            param.isRolling = true;
            param.canMove = false;
            param.rollPress = false;
        }

        private void ExitRoll(PlayerMotion obj)
        {
            param.canMove = true;
            param.isRolling = false;
        }

#endregion
        
    }
}