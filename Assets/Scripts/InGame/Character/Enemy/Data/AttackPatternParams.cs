using System;
using UnityEngine;

namespace InGame.Character
{
    [Serializable]
    public class AttackPatternParams
    {
        public PatternType Type;
        [SerializeField] private AnimationClip _animationClip;
        [Range(0f, 1f)] public float OccurrenceRate;
        public AttackPattern AttackPattern;
        
        public float AttackImpactWaitTime(int index = 0)
        {
            if (_animationClip == null) return 0f;
            
            var events = _animationClip.events;
            if (events.Length <= 0) return 0f;

            return events[index].time;
        }
    }

    public enum PatternType
    {
        Physical, LongRange
    }
}