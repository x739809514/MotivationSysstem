using AnimSystem.Core;

namespace MotionCore
{
    /// <summary>
    /// middle stuff
    /// </summary>
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
            playerAnim = new PlayerAnim(setting,this);
            playerModel = new PlayerModel(this,setting);
            

            playerParam.jumpHandle += playerModel.SwitchToJump;
            playerParam.moveHandle += playerModel.SwitchToMove;
            playerParam.idleHandle += playerModel.SwitchToIdle;
            playerParam.landhandle += playerModel.SwitchToLand;
        }
    }
}