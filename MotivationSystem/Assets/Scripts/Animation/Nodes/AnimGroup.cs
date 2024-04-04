using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimSystem.Core
{
    public class AnimGroup : AnimBehaviour
    {
        private AnimationMixerPlayable mixerPlayable;
        private int _curIndex;
        private int clipCount;
        private double curClipTime;
        private int curIndex
        {
            get => _curIndex;
            set
            {
                _curIndex = value;
                if (_curIndex<clipCount)
                {
                    curClipTime = mixerPlayable.GetInput(value).GetDuration();
                }
            }
        }


        public AnimGroup(PlayableGraph graph) : base(graph)
        {
            mixerPlayable = AnimationMixerPlayable.Create(graph);
            animAdapter.AddInput(mixerPlayable, 0, 1.0f);
        }

        protected override void AddInput(Playable playable)
        {
            base.AddInput(playable);
            mixerPlayable.AddInput(playable, 0, 0f);
            clipCount++;

            if (clipCount == 1)
            {
                mixerPlayable.SetInputWeight(0, 1f);
                curIndex = 0;
            }
        }


        public override void Execute(Playable playable, FrameData info)
        {
            if (enabled == false) return;

            if (curIndex >= clipCount)
            {
                return;
            }
            
            if (curClipTime > 0)
            {
                curClipTime -= info.deltaTime;
            }
            else
            {
                curIndex++;
            }
        }

        public override void Enable()
        {
            mixerPlayable.SetInputWeight(curIndex,1.0f);
            animAdapter.SetTime(0);
            animAdapter.Play();
            mixerPlayable.SetTime(0);
            mixerPlayable.Play();
        }

        public override void Disable()
        {
            base.Disable();
            for (int i = 0; i < clipCount; i++)
            {
                mixerPlayable.SetInputWeight(i,0f);
                AnimHelper.Disable(mixerPlayable.GetInput(i));
            }
            mixerPlayable.Pause();
            animAdapter.Pause();
        }
    }
}