using System.IO;
using FSM;
using Tool;
using UnityEngine;

namespace MotionCore
{
    public class PlayerAI
    {
        private StateManager<PlayerMotion> stateManager;
        private PlayerParam param;
        
        public FSMStateNode<PlayerMotion> idle { get; }
        public FSMStateNode<PlayerMotion> walk { get; }
        public FSMStateNode<PlayerMotion> run { get; }
        public FSMStateNode<PlayerMotion> jump { get; }
        public FSMStateNode<PlayerMotion> fall { get; }

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

            fall = new FSMStateNode<PlayerMotion>("falltoland");
            stateManager.AddState(fall);
            
            var idleCondition = new FSMConditionNode<PlayerMotion>(p => true,1000);
            var moveInput = new FSMConditionNode<PlayerMotion>(p => param.inputVal.magnitude>0, 1001);
            var jumpInput = new FSMConditionNode<PlayerMotion>(p => param.jumpPress, 1002);
            var runInput = new FSMConditionNode<PlayerMotion>(p => param.runPress, 1003);
            var fallInput = new FSMConditionNode<PlayerMotion>(p => param.velocity.y > 0, 1004);

            idle.AddConditions(idleCondition);
            walk.AddConditions(moveInput);
            run.AddConditions(runInput);
            jump.AddConditions(jumpInput);
            fall.AddConditions(fallInput);
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