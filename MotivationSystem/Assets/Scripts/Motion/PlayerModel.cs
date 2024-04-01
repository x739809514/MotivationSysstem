using AnimSystem.Core;
using Tool;
using UnityEngine;

namespace MotionCore
{
    public class PlayerModel
    {
        private PlayerMotion motion;
        private AnimSetting setting;
        private PlayerAnim anim;
        private PlayerParam param;
        private PlayerAI ai;
        private Rigidbody rb;
        private Transform model;
        private float jumpForce;
        private float runForce;
        private float walkForce;
        private float rotateForce;
        private float animMultiply = 1f;
        private float speedMultiply = 1f;


        public PlayerModel(PlayerMotion motion, AnimSetting setting)
        {
            this.motion = motion;
            ai = motion.playerAI;
            anim = motion.playerAnim;
            param = motion.playerParam;
            this.setting = setting;
            model = GameLoop.instance.model;
            jumpForce = GameLoop.instance.jumpForce;
            runForce = GameLoop.instance.runForce;
            walkForce = GameLoop.instance.walkForce;
            rotateForce = GameLoop.instance.rotateSpeed;
            Init();
        }

        private void Init()
        {
            anim.TransitionTo(AnimName.Idle);
            ai.SwitchState(ai.idle);
        }

        public void SwitchToJump()
        {
            anim.TransitionTo(AnimName.Jump);
            ai.SwitchState(ai.jump);
            //Todo: model motion
            GameLoop.instance.rb.velocity += Vector3.up * jumpForce;
        }

        public void SwitchToMove(Vector2 input)
        {
            Debug.Log("Is Moving");
            var moveClip = setting.GetAnim(AnimName.Move).blendClips[0];
            if (param.runPress && input.y > 0)
            {
                Debug.Log("Run");
                ai.SwitchState(ai.run);
                animMultiply = Mathf.Clamp(UpdateMultiply(animMultiply, 5f), 1f, 2f);
                speedMultiply = Mathf.Clamp(UpdateMultiply(speedMultiply, 5f), walkForce, runForce);
            }
            else
            {
                Debug.Log("Walk");
                ai.SwitchState(ai.walk);
                animMultiply = Mathf.Clamp(UpdateMultiply(this.animMultiply, -5f), 1f, 2f);
                speedMultiply = Mathf.Clamp(UpdateMultiply(speedMultiply, -5f), walkForce, runForce);
            }

            anim.UpdateMove(moveClip.pos.x, moveClip.pos.y * animMultiply);
            rb.velocity = new Vector3(model.forward.x * input.y * speedMultiply, rb.velocity.y,
                model.forward.z * input.y * speedMultiply);
            param.velocity = rb.velocity;
            model.Rotate(Vector3.up * input.x * rotateForce * Time.deltaTime);
        }

        private float UpdateMultiply(float value, float speed)
        {
            return value + speed * Time.deltaTime;
        }
    }
}