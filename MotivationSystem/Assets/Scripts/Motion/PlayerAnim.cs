﻿using System.Collections.Generic;
using AnimSystem.Core;
using Tool;
using UnityEngine;
using UnityEngine.Playables;

namespace MotionCore
{
    /// <summary>
    /// Animation of player
    /// Add animation for player
    /// </summary>
    public class PlayerAnim
    {
        private AnimUnit idle;
        private BlendTree2D move;
        private AnimGroup block;
        private AnimUnit roll;
        private AnimUnit execution;
        private PlayableGraph graph;
        private Mixer mixer;
        private Dictionary<string, int> animIndexDics;
        private AnimSetting curSetting;

        public PlayerAnim(AnimSetting setting, string name, PlayerMotion motion)
        {
            graph = PlayableGraph.Create(name);
            mixer = new Mixer(graph);
            curSetting = setting;

            AnimHelper.SetOutPut(graph, mixer, GameLoop.instance.animator);
            AnimHelper.Go(graph, mixer);
        }

        public void LoadAttackAnimation()
        {
            var idleAnim = curSetting.GetAnim(AnimName.Idle);
            idle = new AnimUnit(graph, idleAnim.clip, idleAnim.enterTime);
            AddStateAnim(AnimName.Idle, idle);

            var moveAnim = curSetting.GetAnim(AnimName.Move);
            move = new BlendTree2D(graph, moveAnim.enterTime, moveAnim.blendClips);
            AddStateAnim(AnimName.Move, move);

            var blockAnim = curSetting.GetAnim(AnimName.Block);
            block = new AnimGroup(graph, blockAnim.enterTime);
            foreach (var clip in blockAnim.groupClips)
            {
                block.AddInput(clip, 0.1f);
            }
            AddStateAnim(AnimName.Block, block);

            var executionAnim = curSetting.GetAnim(AnimName.Execution);
            if (executionAnim!=null)
            {
                execution = new AnimUnit(graph, executionAnim.clip, executionAnim.enterTime);
                AddStateAnim(AnimName.Execution,execution);
            }

            var rollAnim = curSetting.GetAnim(AnimName.Roll);
            roll = new AnimUnit(graph, rollAnim.clip, rollAnim.enterTime, false);
            AddStateAnim(AnimName.Roll, roll);

            for (int i = 0; i < curSetting.attacks.Count; i++)
            {
                var anim = curSetting.attacks[i];
                var alv = new AnimUnit(graph, anim.clip, anim.enterTime);
                AddStateAnim(anim.name, alv);
            }
        }

        private void AddStateAnim(string animName, AnimBehaviour behaviour)
        {
            if (animIndexDics == null)
            {
                animIndexDics = new Dictionary<string, int>();
            }

            mixer.AddInput(behaviour);
            if (animIndexDics.ContainsKey(animName) == false)
            {
                animIndexDics.Add(animName, animIndexDics.Count);
            }
        }

        public void TransitionTo(string animName)
        {
            if (animIndexDics.TryGetValue(animName, out var index))
            {
                mixer.TransitionTo(index);
            }
        }

        public void UpdateMove(float x, float y)
        {
            Debug.Log(move.enabled);
            if (move.enabled)
            {
                move.SetPoint(x, y);
            }
        }

        public void StopGraph()
        {
            mixer.TransitToDefault();
            graph.Stop();
        }

        public void RePlayGraph()
        {
            AnimHelper.SetOutPut(graph, mixer, GameLoop.instance.animator);
            AnimHelper.Go(graph, mixer);
        }

        public void OnDestroy()
        {
            move.Destroy();
            graph.Destroy();
        }
    }
}