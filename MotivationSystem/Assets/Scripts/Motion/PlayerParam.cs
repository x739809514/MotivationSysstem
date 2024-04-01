using System;
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


#region Property

        public bool onGround { get; }

        public bool jumpPress
        {
            get => _jumpPress;
            set
            {
                _jumpPress = value;
                if (_jumpPress)
                {
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

                Debug.Log(Math.Abs(_inputMove.x - value.x) + "___" + Math.Abs(_inputMove.y - value.y));
                _inputMove = value;
                moveHandle?.Invoke(value);
            }
        }

#endregion
    }
}