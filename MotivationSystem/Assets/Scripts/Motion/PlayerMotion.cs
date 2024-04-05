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
        
        public PlayerMotion(AnimSetting setting)
        {
            playerParam = new PlayerParam();
            playerAI = new PlayerAI(this);
            playerAnim = new PlayerAnim(setting);
            playerModel = new PlayerModel(this,setting);
            

            playerParam.jumpHandle += playerModel.SwitchToJump;
            playerParam.moveHandle += playerModel.SwitchToMove;
            playerParam.idleHandle += playerModel.SwitchToIdle;
            playerParam.landhandle += playerModel.SwitchToLand;
        }
    }
}