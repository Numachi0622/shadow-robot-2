using System;
using UnityEngine;

namespace InGame.Character
{
    [Serializable]
    public class AttackPatternParams  
    {
        public PatternType Type;
        public string AnimationTrigger;
        public float AnimationStartFrame;
        [Range(0f, 1f)] public float OccurrenceRate;
        public AttackPattern AttackPattern;
    }
    
    public enum PatternType 
    {
        Physical, LongRange
    }
}