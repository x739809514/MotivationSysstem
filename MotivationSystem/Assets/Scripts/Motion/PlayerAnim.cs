using AnimSystem.Core;
using UnityEngine.Playables;

namespace MotionCore
{
    public class PlayerAnim
    {
        public AnimUnit idle;
        public BlendTree2D move;
        public AnimUnit jump;
        private PlayableGraph graph;
        
        public PlayerAnim(AnimSetting setting)
        {
            graph = new PlayableGraph();

            idle = new AnimUnit(graph, setting.GetAnim(Type.Idle));
            //move = new BlendTree2D(graph)
        }
    }
}