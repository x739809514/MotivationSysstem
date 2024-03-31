using FSM;
using UnityEngine;

namespace MotionCore
{
    public class PlayerAI
    {
        private StateManager<PlayerMotion> stateManager;

        public string curStateName => stateManager.curState.stateName;

        public PlayerAI(PlayerMotion motion)
        {
            stateManager = new StateManager<PlayerMotion>(motion);

            var idle = new FSMStateNode<PlayerMotion>("idle");
            idle.BindEnterHandle(p =>
            {
                //Todo:player idle motion
            });
            stateManager.AddState(idle);
            stateManager.SetDefaultState("idle");

            var walk = new FSMStateNode<PlayerMotion>("walk");
            walk.BindUpdateHandle(p =>
            {
                //Todo: player walk motion
            });
            stateManager.AddState(walk);

            var run = new FSMStateNode<PlayerMotion>("run");
            run.BindUpdateHandle(p =>
            {
                //Todo: player run motion
            });
            stateManager.AddState(run);

            var jump = new FSMStateNode<PlayerMotion>("jump");
            jump.BindEnterHandle(p =>
            {
                //Todo: player jumo motion
            });
            stateManager.AddState(jump);

            // Todo: Just Test, need update later
            var moveInput = new FSMConditionNode<PlayerMotion>(p => { return Input.GetKeyDown("Horizontal"); }, 1001);
            var jumpInput = new FSMConditionNode<PlayerMotion>(p => { return Input.GetKeyDown(KeyCode.J); }, 1002);
            var runInput =
                new FSMConditionNode<PlayerMotion>(p => { return Input.GetKeyDown(KeyCode.LeftShift); }, 1003);

            walk.AddConditions(moveInput);
            run.AddConditions(runInput);
            jump.AddConditions(jumpInput);
        }
    }
}