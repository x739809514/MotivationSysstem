using System.Collections.Generic;
using AnimSystem.Core;
using Tool;
using UnityEngine.Playables;

namespace MotionCore
{
    public class PlayerAnim
    {
        private AnimUnit idle;
        private BlendTree2D move;
        private AnimUnit jump;
        private BlendTree2D fall;
        private PlayableGraph graph;
        private Mixer mixer;
        private Dictionary<string, int> animIndexDics;
        
        public PlayerAnim(AnimSetting setting)
        {
            graph = PlayableGraph.Create();
            mixer = new Mixer(graph);
            
            var idleAnim = setting.GetAnim(AnimName.Idle);
            idle = new AnimUnit(graph, idleAnim.clip,idleAnim.enterTime);
            AddStateAnim(AnimName.Idle,idle);
            
            var moveAnim = setting.GetAnim(AnimName.Move);
            move = new BlendTree2D(graph, moveAnim.enterTime, moveAnim.blendClips);
            AddStateAnim(AnimName.Move,move);

            var jumpAnim = setting.GetAnim(AnimName.Jump);
            jump = new AnimUnit(graph, jumpAnim.clip,jumpAnim.enterTime);
            AddStateAnim(AnimName.Jump,jump);

            var fallAnim = setting.GetAnim(AnimName.Fall);
            fall = new BlendTree2D(graph,fallAnim.enterTime,fallAnim.blendClips);
            AddStateAnim(AnimName.Fall,fall);
            
            AnimHelper.SetOutPut(graph,mixer,GameLoop.instance.animator);
            AnimHelper.Go(graph,mixer);
        }

        private void AddStateAnim(string animName, AnimBehaviour behaviour)
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