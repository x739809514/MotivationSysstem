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
        private Vector2 inputVal = Vector2.zero;
        public Vector3 velocity;
        
        // attack
        private int attackLevel;
        
        // move
        public bool runPress;
        public bool inputPress;
        public bool canMove;
        
        // block
        public bool blockPress;
        public bool isExecution;
        public bool isBlocking;
        
        // roll
        public bool rollPress;
        public bool isRolling;
        

        // event handle
        public Action<Vector2> moveHandle;
        public Action idleHandle;
        public Action landhandle;
        public Action<int> attackHandle;
        public Action blockHandle;
        public Action rollHandle;
        public Action executionHandle;


#region Property

        public bool OnGround
        {
            get => onGround;
            set
            {
                onGround = value;
                if (onGround)
                {
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

        public Vector2 InputVal
        {
            get => inputVal;
            set
            {
                inputVal = value;
            }
        }

        public int AttackLevel
        {
            get => attackLevel;
            set
            {
                attackLevel = value;
            }
        }

#endregion
    }
}