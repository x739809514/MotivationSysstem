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
        public PlayerAnim curAnim { get; private set; }
        public PlayerParam playerParam { get; }
        private AttackType curAttackType;
        private PlayerModel playerModel { get; }
        private PlayerAnim attackAnim;
        private PlayerAnim swordAnim;

        public PlayerMotion(AnimSetting setting, AnimSetting setting2)
        {
            playerParam = new PlayerParam();
            playerAI = new PlayerAI(this);

            attackAnim = new PlayerAnim(setting, "Riot", this);
            attackAnim.LoadAttackAnimation();
            swordAnim = new PlayerAnim(setting2, "sword", this);
            swordAnim.LoadAttackAnimation();
            
            curAnim = swordAnim;

            playerModel = new PlayerModel(this, setting);
            curAttackType = AttackType.Null;

            playerParam.moveHandle += playerModel.SwitchToMove;
            playerParam.idleHandle += playerModel.SwitchToIdle;
            playerParam.landhandle += playerModel.SwitchToLand;
            playerParam.attackHandle += playerModel.SwitchToAttack;
            playerParam.blockHandle += playerModel.SwitchToBlock;
        }

        public void LoadRiotAttack()
        {
            if (curAttackType != AttackType.Riot)
            {
                curAttackType = AttackType.Riot;
                curAnim.StopGraph();
                curAnim = attackAnim;
                playerModel.SetCurAnim();
                curAnim.RePlayGraph();
            }
        }

        public void LoadSwordAttack()
        {
            if (curAttackType != AttackType.Sword)
            {
                curAttackType = AttackType.Sword;
                curAnim.StopGraph();
                curAnim = swordAnim;
                playerModel.SetCurAnim();
                curAnim.RePlayGraph();
            }
        }

        public AttackType GetCurAttackType()
        {
            return curAttackType;
        }

        public void OnDestroy()
        {
            attackAnim.OnDestroy();
            swordAnim.OnDestroy();
        }
    }
}