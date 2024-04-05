﻿using System;
using UnityEngine;

namespace MotionCore
{
    /// <summary>
    /// player parameters
    /// </summary>
    public class PlayerParam
    {
        private bool _onGround;
        private bool _jumpPress;
        private Vector2 _inputVal = Vector2.zero;
        public Vector3 velocity;
        public bool runPress;
        public bool inputPress;
        public Action jumpHandle;
        public Action<Vector2> moveHandle;
        public Action idleHandle;
        public Action landhandle;


#region Property

        public bool onGround
        {
            get => _onGround;
            set
            {
                _onGround = value;
                if (_onGround)
                {
                    jumpPress = false;
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

        public bool jumpPress
        {
            get => _jumpPress;
            set
            {
                _jumpPress = value;
                if (_jumpPress)
                {
                    onGround = false;
                    jumpHandle?.Invoke();
                }
            }
        }

        public Vector2 inputVal
        {
            get => _inputVal;
            set
            {
                _inputVal = value;
                if (inputPress==false) return;
                
                moveHandle?.Invoke(value);
            }
        }

#endregion
    }
}