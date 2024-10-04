using System;
using System.Collections.Generic;
using Tools;
using UnityEngine;


namespace AnimSystem.Core
{
    public enum Type
    {
       Single,
       Group,
       Blend,
    }
    
    [Serializable]
    public class AnimInfo
    {
        public string name = "anim";
        public Type type = Type.Single;
        public float enterTime;
        public AnimationClip clip;
        public AnimationClip[] groupClips;
        public ClipData[] blendClips;
    }
    
    [CreateAssetMenu(fileName = "new anim setting",menuName = "RFrame/Anim/AnimSetting")]
    public class AnimSetting : ScriptableObject
    {
        public List<AnimInfo> anims;
        public List<AnimInfo> attacks;

        public AnimInfo GetAnim(string name)
        {
           return anims.Find(p => p.name == name);
        }
    }
}