using FSM;
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

        public string curStateName => stateManager.curState.stateName;

        public PlayerAI(PlayerMotion motion)
        {
            stateManager = new StateManager<PlayerMotion>(motion);
            param = motion.playerParam;

            idle = new FSMStateNode<PlayerMotion>("idle");
            idle.BindEnterHandle(p =>
            {
                //Todo:player idle motion
            });
            stateManager.AddState(idle);

            walk = new FSMStateNode<PlayerMotion>("walk");
            walk.BindUpdateHandle(p =>
            {
                //Todo: player walk motion
            });
            stateManager.AddState(walk);

            run = new FSMStateNode<PlayerMotion>("run");
            run.BindUpdateHandle(p =>
            {
                //Todo: player run motion
            });
            stateManager.AddState(run);

            jump = new FSMStateNode<PlayerMotion>("jump");
            jump.BindEnterHandle(p =>
            {
                //Todo: player jump motion
            });
            jump.BindExitHandle(p =>
            {
                //Todo: player exit jump motion
            });
            stateManager.AddState(jump);

            // Todo: Just Test, need update later
            var idleCondition = new FSMConditionNode<PlayerMotion>(p => true,1000);
            var moveInput = new FSMConditionNode<PlayerMotion>(p => param.inputMove.magnitude>0, 1001);
            var jumpInput = new FSMConditionNode<PlayerMotion>(p => param.jumpPress, 1002);
            var runInput = new FSMConditionNode<PlayerMotion>(p => param.runPress, 1003);

            idle.AddConditions(idleCondition);
            walk.AddConditions(moveInput);
            run.AddConditions(runInput);
            jump.AddConditions(jumpInput);
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