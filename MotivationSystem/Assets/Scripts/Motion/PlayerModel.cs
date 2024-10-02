using AnimSystem.Core;
using Tool;
using Unity.VisualScripting;
using UnityEngine;

namespace MotionCore
{
    /// <summary>
    /// logic of change state
    /// </summary>
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
        private float animMultiply = 1f;
        private float speedMultiply = 1f;
        private float turnSmoothVal;


        public PlayerModel(PlayerMotion motion, AnimSetting setting)
        {
            this.motion = motion;
            ai = motion.playerAI;
            anim = motion.playerAnim;
            param = motion.playerParam;
            this.setting = setting;
            rb = GameLoop.instance.rb;
            model = GameLoop.instance.model;
            jumpForce = GameLoop.instance.jumpForce;
            runForce = GameLoop.instance.runForce;
            walkForce = GameLoop.instance.walkForce;
        }

        private float UpdateMultiply(float value, float speed)
        {
            return value + speed * Time.deltaTime;
        }


#region Motion Switch

        public void SwitchToIdle()
        {
            if (ai.curStateName != null && ai.curStateName == AnimName.Idle)
            {
                return;
            }

            anim.TransitionTo(AnimName.Idle);
            ai.SwitchState(ai.idle);
        }

        /*public void SwitchToJump()
        {
            if (ai.curStateName != null && ai.curStateName == AnimName.Jump)
            {
                return;
            }

            anim.TransitionTo(AnimName.Jump);
            ai.SwitchState(ai.jump);
            //Todo: model motion
            GameLoop.instance.rb.velocity = param.velocity;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }*/

        public void SwitchToMove(Vector2 input)
        {
            // clear
            if (ExitMoveState(input))
            {
                return;
            }

            var moveClip = setting.GetAnim(AnimName.Move).blendClips[0];
            if (param.runPress)
            {
                // run
                ai.SwitchState(ai.run);
                animMultiply = Mathf.Clamp(UpdateMultiply(animMultiply, 5f), 1f, 2f);
                speedMultiply = Mathf.Clamp(UpdateMultiply(speedMultiply, 5f), walkForce, runForce);
            }
            else
            {
                // walk
                ai.SwitchState(ai.walk);
                animMultiply = Mathf.Clamp(UpdateMultiply(this.animMultiply, -5f), 1f, 2f);
                speedMultiply = Mathf.Clamp(UpdateMultiply(speedMultiply, -5f), walkForce, runForce);
            }

            anim.TransitionTo(AnimName.Move);
            anim.UpdateMove(moveClip.pos.x, moveClip.pos.y * animMultiply);

            if (Camera.main != null)
            {
                var transform = Camera.main.transform;
                Vector3 forward = transform.forward;
                Vector3 right = transform.right;
                forward.y = 0;
                right.y = 0;
                forward.Normalize();
                right.Normalize();

                Vector3 direction = new Vector3(input.x, 0, input.y).normalized;

                var moveDirection = Vector3.zero;
                if (direction.magnitude >= 0.1f)
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg +
                                        Camera.main.transform.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(model.eulerAngles.y, targetAngle, ref turnSmoothVal, 0.1f);
                    model.rotation = Quaternion.Euler(0f, angle, 0f);
                    Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                    moveDirection = moveDir.normalized * speedMultiply;
                }

                Vector3 move = moveDirection * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + move);
                param.velocity = rb.velocity;
            }
        }

        public void SwitchToLand()
        {
            anim.TransitionTo(AnimName.Land);
            ai.SwitchState(ai.land);
        }

        public void SwitchToAttack(int alv)
        {
            if (param.OnGround == false) return;
            switch (alv)
            {
                case 0:
                    Debug.Log("idle");
                    anim.TransitionTo(AnimName.Idle);
                    break;
                case 1:
                    anim.TransitionTo(AnimName.AttackLv1);
                    ai.SwitchState(ai.attack);
                    break;
                case 2:
                    anim.TransitionTo(AnimName.AttackLv2);
                    ai.SwitchState(ai.attack);
                    break;
                case 3:
                    anim.TransitionTo(AnimName.AttackLv3);
                    ai.SwitchState(ai.attack);
                    break;
                case 4:
                    anim.TransitionTo(AnimName.AttackLv4);
                    ai.SwitchState(ai.attack);
                    break;
                case 5:
                    anim.TransitionTo(AnimName.AttackLv5);
                    ai.SwitchState(ai.attack);
                    break;
                case 6:
                    anim.TransitionTo(AnimName.AttackLv6);
                    ai.SwitchState(ai.attack);
                    break;
            }
        }

#endregion


#region Method

        private bool ExitMoveState(Vector2 input)
        {
            if (input == Vector2.zero)
            {
                param.inputPress = false;
                SwitchToIdle();
                rb.velocity = Vector3.zero;
                return true;
            }

            if (param.OnGround == false)
            {
                return true;
            }

            return false;
        }

#endregion
    }
}