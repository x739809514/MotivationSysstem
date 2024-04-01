using System;
using UnityEngine;

namespace MotionCore
{
    public class PlayerParam
    {
        private bool _onGround;
        private bool _jumpPress;
        private bool _movePress;
        public Vector3 velocity;
        public Vector2 inputMove;
        public bool runPress;
        public bool onGround
        {
            get;
        }

        public bool jumpPress
        {
            get
            {
                return _jumpPress;
            }
            set
            {
                _jumpPress = value;
                jumpHandle?.Invoke();
            }
        }
        
        public bool movePress
        {
            get
            {
                return _movePress;
            }
            set
            {
                _movePress = value;
                moveHandle?.Invoke(inputMove);
            }
        }

        public Action jumpHandle;
        public Action<Vector2> moveHandle;
    }
}