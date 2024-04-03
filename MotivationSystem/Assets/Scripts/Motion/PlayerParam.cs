using System;
using UnityEditor;
using UnityEngine;

namespace MotionCore
{
    public class PlayerParam
    {
        private bool _onGround;
        private bool _jumpPress;
        private Vector2 _inputMove = Vector2.zero;
        public Vector3 velocity;
        public bool runPress;
        public Action jumpHandle;
        public Action<Vector2> moveHandle;
        public Action onGroundHandle;


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
                    onGroundHandle?.Invoke();
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

        public Vector2 inputMove
        {
            get => _inputMove;
            set
            {
                if (Math.Abs(_inputMove.x - value.x) < 0.01f && Math.Abs(_inputMove.y - value.y) < 0.01f)
                {
                    return;
                }

                _inputMove = value;
                moveHandle?.Invoke(value);
            }
        }

#endregion
    }
}