using AnimSystem.Core;
using Tool;

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
        private AttackType curAttackType;
        
        public PlayerMotion(AnimSetting setting)
        {
            playerParam = new PlayerParam();
            playerAI = new PlayerAI(this);
            playerAnim = new PlayerAnim(setting,this);
            playerModel = new PlayerModel(this,setting);
            curAttackType = AttackType.Null;
            
            playerParam.moveHandle += playerModel.SwitchToMove;
            playerParam.idleHandle += playerModel.SwitchToIdle;
            playerParam.landhandle += playerModel.SwitchToLand;
            playerParam.attackHandle += playerModel.SwitchToAttack;
        }

        public void LoadRiotAttack(AnimSetting setting)
        {
            if (curAttackType!=AttackType.Riot)
            {
                curAttackType = AttackType.Riot;
                playerAnim.LoadAttackAnimation(setting);
            }
        }

        public AttackType GetCurAttackType()
        {
            return curAttackType;
        }

        public void OnDestroy()
        {
            playerAnim.OnDestroy();
        }
    }
}