using AnimSystem.Core;
using Tool;

namespace MotionCore
{
    public class PlayerModel
    {
        private PlayerMotion motion;
        private AnimSetting setting;
        private PlayerAI ai;
        
        public PlayerModel(PlayerMotion motion,AnimSetting setting)
        {
            this.motion = motion;
            ai = motion.playerAI;
            this.setting = setting;
        }

        public void SwitchToJump()
        {
            motion.playerAnim.TransitionTo(AnimName.Jump);
            ai.SwitchState(ai.jump);
            //Todo: model motion
        }

        public void SwitchToMove()
        {
            var moveClip = setting.GetAnim(AnimName.Move).blendClips[0];
            motion.playerAnim.UpdateMove(moveClip.pos.x,moveClip.pos.y);
            ai.SwitchState(ai.walk);
            //Todo: model motion
        }

        public void SwitchToRun()
        {
            var runClip = setting.GetAnim(AnimName.Move).blendClips[1];
            motion.playerAnim.UpdateMove(runClip.pos.x,runClip.pos.y);
            ai.SwitchState(ai.run);
            //Todo: model motion
        }
    }
}