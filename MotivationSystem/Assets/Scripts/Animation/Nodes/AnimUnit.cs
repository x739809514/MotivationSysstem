using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimSystem.Core
{
    public class AnimUnit : AnimBehaviour
    {
        private AnimationClipPlayable clipPlayable;
        private AnimationClip clip;
        //private float curClipTime;
        private float declineSpeed = 2f;

        private bool interrupt;
        //private double animStopTime;

        public AnimUnit(PlayableGraph graph, AnimationClip clip, float enterTime = 0f, bool canInterrupt = true) : base(
            graph, enterTime)
        {
            this.clip = clip;
            clipPlayable = AnimationClipPlayable.Create(graph, clip);
            animAdapter.AddInput(clipPlayable, 0, 1.0f);
            //curClipTime = clip.length;
            interrupt = canInterrupt;
            Disable();
        }

        public override void Execute(Playable playable, FrameData info)
        {
            base.Execute(playable, info);
            if (enabled == false) return;

            /*if (curClipTime > 0f)
            {
                curClipTime -= info.deltaTime * declineSpeed;
            }
            else
            {
                callback?.Invoke();
            }*/
            if (remainTime<=0)
            {
                callback?.Invoke();
                Disable();
            }
        }


        public override void Enable()
        {
            base.Enable();
            animAdapter.SetTime(0f);
            clipPlayable.SetTime(0f);
            animAdapter.Play();
            clipPlayable.Play();
        }

        public override void Disable()
        {
            base.Disable();
            animAdapter.Pause();
            clipPlayable.Pause();
            //animStopTime = animAdapter.GetTime();
        }

        public override float GetAnimLength()
        {
            return clip.length;
        }

        public float GetClipRemainTime()
        {
            return remainTime;
        }

        public bool CanAnimInterrupt()
        {
            return interrupt;
        }
    }
}