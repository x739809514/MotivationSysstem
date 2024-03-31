using AnimSystem.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace MotionCore
{
    public class IdleAnim : AnimUnit
    {
        private AnimationClip clip;
        public IdleAnim(PlayableGraph graph, AnimationClip clip, float enterTime = 0) : base(graph, clip, enterTime)
        {
            this.clip = clip;
        }
        
        
    }
}