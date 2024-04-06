using System.Collections.Generic;
using AnimSystem.Core;
using Tool;
using UnityEngine;
using UnityEngine.Playables;

namespace MotionCore
{
    /// <summary>
    /// Animation of player
    /// Add animattion for player
    /// </summary>
    public class PlayerAnim
    {
        private AnimUnit idle;
        private BlendTree2D move;
        private AnimUnit jump;
        private AnimGroup land;
        private PlayableGraph graph;
        private Mixer mixer;
        private Dictionary<string, int> animIndexDics;
        
        public PlayerAnim(AnimSetting setting,PlayerMotion motion)
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

            var landAnim = setting.GetAnim(AnimName.Land);
            land = new AnimGroup(graph,0.1f);
            land.BindCallBackHandle(() =>
            {
                motion.playerAI.SwitchState(motion.playerAI.idle);
            });
            AddGroupAnim(landAnim.groupClips,landAnim.enterTime);
            AddStateAnim(AnimName.Land,land);
            
            AnimHelper.SetOutPut(graph,mixer,GameLoop.instance.animator);
            AnimHelper.Go(graph,mixer);
        }

        private void AddGroupAnim(AnimationClip[] clips, float enterTime)
        {
            for (int i = 0,length = clips.Length; i < length; i++)
            {
                land.AddInput(clips[i],enterTime);
            }
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

        public void OnDestroy()
        {
            graph.Destroy();
        }
    }
}