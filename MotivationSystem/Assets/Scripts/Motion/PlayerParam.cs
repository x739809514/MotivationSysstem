using System;
using UnityEngine;

namespace MotionCore
{
    /// <summary>
    /// player parameters
    /// </summary>
    public class PlayerParam
    {
        private bool onGround;
        private bool jumpPress;
        private int attackLevel;
        private Vector2 inputVal = Vector2.zero;
        public Vector3 velocity;
        public bool runPress;
        public bool inputPress;
        public Action jumpHandle;
        public Action<Vector2> moveHandle;
        public Action idleHandle;
        public Action landhandle;
        public Action<int> attackHandle;


#region Property

        public bool OnGround
        {
            get => onGround;
            set
            {
                onGround = value;
                if (onGround)
                {
                    JumpPress = false;
                    if (velocity.y<0)
                    {
                        landhandle?.Invoke();
                    }
                    else
                    {
                        idleHandle?.Invoke();
                    }
                }
            }
        }

        public bool JumpPress
        {
            get => jumpPress;
            set
            {
                jumpPress = value;
                if (jumpPress)
                {
                    OnGround = false;
                    jumpHandle?.Invoke();
                }
            }
        }

        public Vector2 InputVal
        {
            get => inputVal;
            set
            {
                inputVal = value;
                if (inputPress==false) return;
                
                moveHandle?.Invoke(value);
            }
        }

        public int AttackLevel
        {
            get => attackLevel;
            set
            {
                attackLevel = value;
                attackHandle?.Invoke(attackLevel);
            }
        }

#endregion
    }
}