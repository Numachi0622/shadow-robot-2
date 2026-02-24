using UnityEngine.Playables;

namespace InGame.Event
{
    public interface ICutSceneContext
    {
        public PlayableDirector Director { get; set; }
    }
}