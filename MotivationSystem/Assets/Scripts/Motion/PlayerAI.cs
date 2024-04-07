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
        public FSMStateNode<PlayerMotion> jump { get; }
        public FSMStateNode<PlayerMotion> land { get; }
        public FSMStateNode<PlayerMotion> attack { get; }

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

            jump = new FSMStateNode<PlayerMotion>("jump");
            stateManager.AddState(jump);

            land = new FSMStateNode<PlayerMotion>("land");
            stateManager.AddState(land);

            attack = new FSMStateNode<PlayerMotion>("attack");
            stateManager.AddState(attack);
            
            var idleCondition = new FSMConditionNode<PlayerMotion>(p => true,1000);
            var moveInput = new FSMConditionNode<PlayerMotion>(p => param.InputVal.magnitude>0, 1001);
            var jumpInput = new FSMConditionNode<PlayerMotion>(p => param.JumpPress, 1002);
            var runInput = new FSMConditionNode<PlayerMotion>(p => param.runPress, 1003);
            var landInput = new FSMConditionNode<PlayerMotion>(p => param.velocity.y < 0, 1004);
            var attackInput = new FSMConditionNode<PlayerMotion>(p => param.AttackLevel >= 1, 1005);
            
            idle.AddConditions(idleCondition);
            walk.AddConditions(moveInput);
            run.AddConditions(runInput);
            jump.AddConditions(jumpInput);
            land.AddConditions(landInput);
            attack.AddConditions(attackInput);
        }

        public void Update()
        {
            stateManager.UpdateState();
        }

        public void SwitchState(FSMStateNode<PlayerMotion> state)
        {
            stateManager.SwitchState(state);
        }
    }
}