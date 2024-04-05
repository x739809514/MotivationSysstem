using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimSystem.Core
{
    /// <summary>
    /// use for animations which play one by one
    /// </summary>
    public class AnimGroup : AnimBehaviour
    {
        private AnimationMixerPlayable mixerPlayable;
        private int _curIndex;
        private int lastIndex;
        private int clipCount;
        private float curClipTime;
        private float declineSpeed = 2f;

        private int curIndex
        {
            get => _curIndex;
            set
            {
                _curIndex = value;
                if (_curIndex < clipCount)
                {
                    var adapter = AnimHelper.GetAdapter(mixerPlayable.GetInput(_curIndex));
                    curClipTime = adapter.animBehaviour.GetAnimLength();
                }
            }
        }


        public AnimGroup(PlayableGraph graph, float enterTime) : base(graph, enterTime)
        {
            mixerPlayable = AnimationMixerPlayable.Create(graph);
            animAdapter.AddInput(mixerPlayable, 0, 1.0f);
            Disable();
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
                lastIndex = curIndex - 1;
            }
        }


        public override void Execute(Playable playable, FrameData info)
        {
            base.Execute(playable, info);
            if (enabled == false) return;
            if (curIndex >= clipCount)
            {
                return;
            }
            
            if (curClipTime > 0f)
            {
                curClipTime -= info.deltaTime * declineSpeed;
            }
            else
            {
                if (curIndex + 1 >= clipCount) return;
                lastIndex = curIndex;
                curIndex += 1;
                Transition(lastIndex, curIndex);
            }
        }

        public override void Enable()
        {
            base.Enable();
            mixerPlayable.SetInputWeight(curIndex, 1.0f);
            animAdapter.SetTime(0);
            animAdapter.Play();
            mixerPlayable.SetTime(0);
            mixerPlayable.Play();
            AnimHelper.Enable(mixerPlayable.GetInput(0));
        }

        public override void Disable()
        {
            base.Disable();
            for (int i = 0; i < clipCount; i++)
            {
                mixerPlayable.SetInputWeight(i, 0f);
                AnimHelper.Disable(mixerPlayable.GetInput(i));
            }

            mixerPlayable.Pause();
            animAdapter.Pause();
        }

        private void Transition(int last, int cur)
        {
            mixerPlayable.SetInputWeight(last, 0f);
            mixerPlayable.SetInputWeight(cur, 1.0f);
            AnimHelper.Disable(mixerPlayable.GetInput(last));
            AnimHelper.Enable(mixerPlayable.GetInput(cur));
        }
    }
}