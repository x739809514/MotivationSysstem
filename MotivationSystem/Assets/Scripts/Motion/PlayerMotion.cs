using AnimSystem.Core;
using Tool;

namespace MotionCore
{
    public class PlayerMotion
    {
        public PlayerAI playerAI { get; }
        public PlayerAnim playerAnim { get; }
        public PlayerModel playerModel { get; }
        public PlayerParam playerParam { get; }
        private AnimSetting setting;
        
        public PlayerMotion(AnimSetting setting)
        {
            this.setting = setting;
            playerAI = new PlayerAI(this);
            playerAnim = new PlayerAnim(setting);
            playerModel = new PlayerModel(this,setting);
            playerParam = new PlayerParam();

            playerParam.jumpHandle += playerModel.SwitchToJump;
            playerParam.moveHandle += playerModel.SwitchToMove;
        }
    }
}