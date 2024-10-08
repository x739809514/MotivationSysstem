using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimSystem.Core
{
    public class Mixer : AnimBehaviour
    {
        private AnimationMixerPlayable mixerPlayable;
        private List<int> declineIndex;
        private int curIndex;
        private static int targetIndex;
        private int clipCount;
        private bool isTransition;
        private float normalSpeed;
        private float declineSpeed;
        private float timeToNext;
        private float declineWeight;

        public Mixer(PlayableGraph graph) : base(graph)
        {
            mixerPlayable = AnimationMixerPlayable.Create(graph);
            animAdapter.AddInput(mixerPlayable, 0, 1.0f);
            declineIndex = new List<int>();
            targetIndex = -1;
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

        public override void Enable()
        {
            base.Enable();

            if (clipCount > 0)
            {
                AnimHelper.Enable(mixerPlayable.GetInput(0));
            }

            mixerPlayable.SetTime(0f);
            mixerPlayable.Play();
            animAdapter.SetTime(0f);
            animAdapter.Play();

            animAdapter.SetInputWeight(0, 1f);

            curIndex = 0;
            targetIndex = -1;
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

        public override void Execute(Playable playable, FrameData info)
        {
            base.Execute(playable, info);
            if (enabled == false) return;
            if (isTransition == false || targetIndex < 0) return;
            if (timeToNext > 0f)
            {
                declineWeight = 0f;
                timeToNext -= info.deltaTime;
                for (int i = declineIndex.Count - 1; i >= 0; i--)
                {
                    var w = ModifyWeight(declineIndex[i], -info.deltaTime * declineSpeed);
                    if (w <= 0)
                    {
                        AnimHelper.Disable(mixerPlayable.GetInput(declineIndex[i]));
                        declineIndex.RemoveAt(i);
                    }
                    else
                    {
                        declineWeight += w;
                    }
                }

                declineWeight += ModifyWeight(curIndex, -info.deltaTime * normalSpeed);
                SetInputWeight(targetIndex, 1.0f - declineWeight);
                return;
            }

            isTransition = false;
            AnimHelper.Disable(mixerPlayable.GetInput(curIndex));
            curIndex = targetIndex;
            targetIndex = -1;
        }


        // cur mixer is interrupt in default
        public void TransitionTo(int index)
        {
            if (GetCurClipRemain() > 0)
            {
                return;
            }

            if (isTransition && targetIndex >= 0)
            {
                if (index == targetIndex) return;
                if (index == curIndex)
                {
                    curIndex = targetIndex;
                }
                else if (GetWeight(curIndex) > GetWeight(targetIndex))
                {
                    declineIndex.Add(targetIndex);
                }
                else
                {
                    declineIndex.Add(curIndex);
                    curIndex = targetIndex;
                }
            }
            else
            {
                if (curIndex == index) return;
            }

            targetIndex = index;
            // if new clip is already in decline array, move it out
            declineIndex.Remove(targetIndex);
            AnimHelper.Enable(mixerPlayable.GetInput(targetIndex));
            timeToNext = GetAnimEnterTime(targetIndex) * (1.0f - GetWeight(targetIndex));
            normalSpeed = GetWeight(curIndex) / timeToNext;
            declineSpeed = 2.0f / timeToNext;
            isTransition = true;
        }

        private float GetWeight(int index)
        {
            if (index < 0 || index >= clipCount) return 0;
            var w = mixerPlayable.GetInputWeight(index);
            return w;
        }

        private float GetAnimEnterTime(int index)
        {
            return ((ScriptPlayable<AnimAdapter>)mixerPlayable.GetInput(index)).GetBehaviour().GetAnimEnterTime();
        }

        private float ModifyWeight(int index, float delta)
        {
            if (index < 0 || index >= clipCount) return 0;
            var weight = Mathf.Clamp01(mixerPlayable.GetInputWeight(index) + delta);
            mixerPlayable.SetInputWeight(index, weight);

            // Disable animation if weight is zero
            if (weight <= 0f)
            {
                AnimHelper.Disable(mixerPlayable.GetInput(index));
            }

            return weight;
        }


        private void SetInputWeight(int index, float weight)
        {
            if (index >= 0 && index < clipCount)
            {
                mixerPlayable.SetInputWeight(index, weight);
            }
        }

        private float GetCurClipRemain()
        {
            var remain = 0f;
            if (curIndex == -1) return 0;
            var behaviour = ((ScriptPlayable<AnimAdapter>)mixerPlayable.GetInput(curIndex)).GetBehaviour()
                .animBehaviour;
            if (behaviour is AnimUnit unit)
            {
                remain = unit.GetClipRemainTime();
                if (unit.CanAnimInterrupt())
                {
                    remain = 0f;
                }
            }

            return remain;
        }

        public float GetCurClipLength()
        {
            var behaviour = ((ScriptPlayable<AnimAdapter>)mixerPlayable.GetInput(targetIndex)).GetBehaviour()
                .animBehaviour;
            if (behaviour is AnimUnit unit)
            {
                return unit.GetAnimLength();
            }

            return 0f;
        }

        public void TransitToDefault()
        {
            for (int i = 0; i < mixerPlayable.GetInputCount(); i++)
            {
                mixerPlayable.SetInputWeight(i, i == 0 ? 1f : 0f);
            }
        }
    }
}