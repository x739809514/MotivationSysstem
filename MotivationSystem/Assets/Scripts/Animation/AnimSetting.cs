using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnimSystem.Core
{
    public enum Type
    {
       Idle,
       Walk,
       Run,
       Jump
    }
    
    [Serializable]
    public class AnimInfo
    {
        public Type type = Type.Idle;
        public float enterTime;
        public AnimationClip clip;
    }
    
    [CreateAssetMenu(fileName = "new anim setting",menuName = "RFrame/Anim/AnimSetting")]
    public class AnimSetting : ScriptableObject
    {
        public List<AnimInfo> anims;

        public AnimationClip GetAnim(Type type)
        {
           return anims.Find(p => p.type == type).clip;
        }
    }
}