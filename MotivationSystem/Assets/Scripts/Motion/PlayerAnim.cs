using System;
using System.Collections.Generic;
using AnimSystem.Core;
using Tool;
using UnityEditor.AnimatedValues;
using UnityEngine.Playables;

namespace MotionCore
{
    public class PlayerAnim
    {
        private AnimUnit idle;
        private BlendTree2D move;
        private AnimUnit jump;
        private PlayableGraph graph;
        private Mixer mixer;
       // private Dictionary<string, AnimBehaviour> animDics;
        private Dictionary<string, int> animIndexDics;
        
        public PlayerAnim(AnimSetting setting)
        {
            graph = new PlayableGraph();
            mixer = new Mixer(graph);
            idle = new AnimUnit(graph, setting.GetAnim(AnimName.Idle).clip);
            AddStateAnim(AnimName.Idle,idle);
            var temp = setting.GetAnim(AnimName.Move);
            move = new BlendTree2D(graph, temp.enterTime, temp.blendClips);
            AddStateAnim(AnimName.Move,move);
            jump = new AnimUnit(graph, setting.GetAnim(AnimName.Jump).clip);
            AddStateAnim(AnimName.Jump,jump);
        }

        public void AddStateAnim(string animName, AnimBehaviour behaviour)
        {
            if (animIndexDics==null)
            {
                animIndexDics = new Dictionary<string, int>();
            }
            mixer.AddInput(behaviour);
            if (animIndexDics.ContainsKey(animName)==false)
            {
                animIndexDics.Add(animName,animIndexDics.Count);
            }
        }

        public void TransitionTo(string animName)
        {
            if (animIndexDics.TryGetValue(animName,out var index))
            {
                mixer.TransitionTo(index);
            }
        }

        public void UpdateMove(float x, float y)
        {
            if (move.enabled)
            {
                move.SetPoint(x,y);
            }
        }
    }
}