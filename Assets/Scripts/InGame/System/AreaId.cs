using System;

namespace InGame.System
{
    [Serializable]
    public struct AreaId : IEquatable<AreaId>
    {
        public readonly int Value;
        public AreaId(int value)
        {
            Value = value;
        }

        public bool Equals(AreaId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is AreaId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}