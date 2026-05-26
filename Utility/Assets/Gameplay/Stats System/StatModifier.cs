using System;
namespace Astek.Stats_System
{
    public enum StatModType
    {
        Flat = 100,
        PercentAdd = 200, //For Stacking Mods
        PercentMult = 300
    }

    public readonly struct StatModifier : IEquatable<StatModifier>
    {
        public readonly float Value;
        public readonly StatModType Type;
        public readonly int Order;
        public readonly object? Source;

        private readonly int _hashCode;

        public StatModifier(float value, StatModType type, int order, object? source)
        {
            Value = value;
            Type = type;
            Order = order;
            Source = source;
            _hashCode = HashCode.Combine(Value, Type, Order, Source);
        }
        public StatModifier(float value, StatModType type) : this(value, type, (int)type, null) { }
        public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }
        public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }

        public bool Equals(StatModifier other) => Value.Equals(other.Value) && Type == other.Type && Order == other.Order && Equals(Source, other.Source);
        public override bool Equals(object obj) => obj is StatModifier other && Equals(other);
        public override int GetHashCode() => _hashCode;
    }
}